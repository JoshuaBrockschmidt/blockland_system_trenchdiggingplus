////
// Mode for digging dirt. Allows you to adjust cube size of section of dirt to dig.
////

TDP_ServerGroup.add(
	new ScriptObject(TRT_ShovelMode)
	{
		class = "TRTMode";
		name  = "Shovel Mode";
	});

function TRT_ShovelMode_getGhostPosition(%client) {
	%eyePoint = %client.player.getEyePoint();
	%eyeVector = %client.player.getEyeVector();
	%pos = vectorAdd(%eyePoint, vectorscale(%eyeVector, $TRT::toolRange));
	%ray = containerRayCast(%eyePoint, %pos, $TypeMasks::FxBrickObjectType);
	%dirt = firstWord(%ray);
	%normal = normalFromRayCast(%ray);
	if(isObject(%dirt) && %dirt.getDatablock().isTrenchDirt) {
		%rayPos = posFromRayCast(%ray);
		%isBrick = TDP_isDirtBrick(%dirt);
		if (%isBrick) {
			%normalZ = mFloor(getWord(%normal, 2) + 0.5);
			// If viewing from top or bottom.
			if (%normalZ != 0) {
				%posXY = getWords(%rayPos, 0, 1) SPC 0;
				if (%client.TRT_cubeSize % 2 == 1) {
					%posXY = vectorScale(%posXY, 2);
					%posXY = vectorFloor(%posXY);
					%posXY = vectorAdd(%posXY, "0.5 0.5 0");
					%posXY = vectorScale(%posXY, 0.5);
				} else {
					%posXY = vectorScale(%posXY, 2);
					%posXY = vectorAdd(%posXY, "0.5 0.5 0");
					%posXY = vectorFloor(%posXY);
					%posXY = vectorScale(%posXY, 0.5);
				}
				%posZ = getWord(%dirt.position, 2) - %normalZ * 0.3 * (%client.TRT_cubeSize - 1);
				%pos = vectorAdd(%posXY, 0 SPC 0 SPC %posZ);
			}
			// If viewing from side.
			else {
				%posXY = getWords(%rayPos, 0, 1) SPC 0;
				%posXY = vectorSub(%posXY, vectorScale(%normal, %client.TRT_cubeSize * 0.25));
				%posZ = getWord(%rayPos, 2);
				if (%client.TRT_cubeSize % 2 == 1) {
					%posXY = vectorScale(%posXY, 2);
					%posXY = vectorFloor(%posXY);
					%posXY = vectorAdd(%posXY, "0.5 0.5 0");
					%posXY = vectorScale(%posXY, 0.5);

					%dirtZ = getWord(%dirt.position, 2);
					%displaceZ = %posZ - %dirtZ;
					%displaceZ = mFloor(%displaceZ * 10/6 + 0.5) * 0.6;
					%posZ = %dirtZ + %displaceZ;
				} else {
					%posXY = vectorScale(%posXY, 2);
					%posXY = vectorAdd(%posXY, "0.5 0.5 0");
					%posXY = vectorFloor(%posXY);
					%posXY = vectorScale(%posXY, 0.5);

					%dirtZ = getWord(%dirt.position, 2);
					%displaceZ = %posZ - %dirtZ;
					%displaceZ = (mFloor(%displaceZ * 10/6) + 0.5) * 0.6;
					%posZ = %dirtZ + %displaceZ;
				}
				
				%pos = vectorAdd(%posXY, 0 SPC 0 SPC %posZ);
			}
			%retArgs = %pos SPC %normal SPC %dirt;
		}
		else if(TDP_isDirtCube(%dirt)) {
			%offGridPos = vectorSub(%rayPos,
						vectorScale(%normal,
							    %client.TRT_cubeSize / 2));
			%displace = vectorSub(%offGridPos, %dirt.position);
			if (((%dirt.getDatablock().brickSizeX+%client.TRT_cubeSize*2-2) % 4) / 2)
				%newDisplace = vectorFloor(vectorAdd(%displace, "0.5 0.5 0.5"));
			else
				%newDisplace = vectorAdd(vectorFloor(%displace),
							 "0.5 0.5 0.5");
			%onGridPos = vectorAdd(%dirt.position, %newDisplace);
			%retArgs = %onGridPos SPC %normal SPC %dirt;
		}
	}
	return %retArgs;
}

function TRT_ShovelMode_ghostLoop(%client) {
	%args = TRT_ShovelMode_getGhostPosition(%client);
	if (%args $= "") {
		if (isObject(%client.TRT_ghostGroup))
			%client.TRT_ghostGroup.delete();
	} else {
		%pos = getWords(%args, 0, 2);
		%dirt = getWord(%args, 6);
		%isBrick = TDP_isDirtBrick(%dirt);
		if (isObject(%client.TRT_ghostGroup) &&
		    %client.TRT_ghostGroup.isBrick == %isBrick) {
			if (%client.TRT_ghostGroup.position !$= %pos)
				%client.TRT_ghostGroup.setTransform(%pos);
		} else {
			if (isObject(%client.TRT_ghostGroup))
				%client.TRT_ghostGroup.delete();
			%newGhost = TRT_ghostGroup(%client, %client.TRT_cubeSize, %pos, %isBrick);
			%client.TRT_ghostGroup = %newGhost;
		}
	}
	if (isObject(%client.TRT_ghostGroup))
	        %client.TRT_ghostGroup.updateColor();
	%client.TRT_updateText();
	%schedID = schedule(100, 0, TRT_ShovelMode_ghostLoop, %client);
	%client.TRT_shovelMode_schedID = %schedID;
}

function TRT_ShovelMode::fire(%this, %client) {
	if (%client.TDP_dirtCnt >= $TDP::maxDirt && !%client.TDP_isInfDirt) {
		%client.centerPrint("\c3You do not have enough room for any more dirt!", 1);
		return;
	}
	%args = TRT_ShovelMode_getGhostPosition(%client);
	if (%args !$= "") {
		// Try to take chunk(s)
		%pos = getWords(%args, 0, 2);
		%normal = getWords(%args, 3, 5);
		%dirt = getWord(%args, 6);
		%isBrick = TDP_isDirtBrick(%dirt);
		%toTake = TDP_Chunker(%client);
		if (%isBrick)
			%box = vectorScale("0.5 0.5 0.6", %client.TRT_cubeSize);
		else
			%box = vectorScale("1 1 1", %client.TRT_cubeSize);
		%box = vectorSub(%box, "0.1 0.1 0.1");
		%toTake.findChunks(%box, %pos);
		%totalTake = %toTake.getTotalTake();
		if (%client.TDP_dirtCnt + %totalTake > $TDP::maxDirt && !%client.TDP_isInfDirt) {
			%needed = (%client.TDP_dirtCnt + %totalTake) - $TDP::maxDirt;
			%msg = "\c3You do not have enough room for that much dirt!\n" @
				 "\c3You need" SPC %needed SPC "less dirt.";
			%client.centerPrint(%msg, 2);
			// TODO: instead of simply restricting player from digging dirt,
			//       start by digging bricks closest to player
		}
		else {
			%colorIDs = %toTake.take();
			if (!%client.TDP_isInfDirt)
				for (%i = 0; %i < %totalTake; %i++)
					%client.TDP_pushDirt(1, getWord(%colorIDs, %i));
		}
		%toTake.delete();
	}
	%client.TRT_updateText();
}

function TRT_ShovelMode::onStartMode(%this, %client) {
	TRT_ShovelMode_ghostLoop(%client);
}

function TRT_ShovelMode::onStopMode(%this, %client) {
	cancel(%client.TRT_shovelMode_schedID);
	if (isObject(%client.TRT_ghostGroup))
		%client.TRT_ghostGroup.delete();
}

function TRT_ShovelMode::getImage(%this, %client) {
	if (%client.TDP_isSpeedDirt)
		return TrenchToolSpeedShovelImage;
	else
		return TrenchToolShovelImage;
}
