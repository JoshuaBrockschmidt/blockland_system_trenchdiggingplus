////
// Mode for digging dirt. Allows you to adjust cube size of section of dirt to dig.
////

// TODO: change speed of digging based on cube size

BTT_ServerGroup.add(
	new ScriptObject(BTT_ShovelMode)
	{
		class = "BTTMode";
		index = $BTT::ShovelMode;
		name  = "Shovel Mode";
	});

function BTT_ShovelMode_getGhostPosition(%client) {
	%eyePoint = %client.player.getEyePoint();
	%eyeVector = %client.player.getEyeVector();
	%pos = vectorAdd(%eyePoint, vectorscale(%eyeVector, $BTT::toolRange));
	%ray = containerRayCast(%eyePoint, %pos, $TypeMasks::FxBrickObjectType);
	%dirt = firstWord(%ray);
	%normal = normalFromRayCast(%ray);
	if(isObject(%dirt) && %dirt.getDatablock().isTrenchDirt) {
		%rayPos = posFromRayCast(%ray);
		%isBrick = BTT_isDirtBrick(%dirt);
		if (%isBrick) {
			%normalZ = mFloor(getWord(%normal, 2) + 0.5);
			// If viewing from top or bottom.
			if (%normalZ != 0) {
				%posXY = getWords(%rayPos, 0, 1) SPC 0;
				if (%client.BTT_cubeSizeBricks % 2 == 1) {
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
				%posZ = getWord(%dirt.position, 2)
					 - %normalZ * 0.3 * (%client.BTT_cubeSizeBricks - 1);
				%pos = vectorAdd(%posXY, 0 SPC 0 SPC %posZ);
			}
			// If viewing from side.
			else {
				%posXY = getWord(%rayPos, 0)
					SPC getWord(%rayPos, 1)
					SPC 0;
				%posXY = vectorSub(%posXY,
						   vectorScale(%normal,
							       %client.BTT_cubeSizeBricks * 0.25));
				%posZ = getWord(%rayPos, 2);
				if (%client.BTT_cubeSizeBricks % 2 == 1) {
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
		else if(BTT_isDirtCube(%dirt)) {
			%offGridPos = vectorSub(%rayPos,
						vectorScale(%normal,
							    %client.BTT_cubeSizeCubes / 2));
			%displace = vectorSub(%offGridPos, %dirt.position);
			if (((%dirt.getDatablock().brickSizeX+%client.BTT_cubeSizeCubes*2-2) % 4) / 2)
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

function BTT_ShovelMode_ghostLoop(%client) {
	%args = BTT_ShovelMode_getGhostPosition(%client);
	if (%args $= "") {
		if (isObject(%client.BTT_ghostGroup))
			%client.BTT_ghostGroup.delete();
		%client.BTT_dirtType = "";
	} else {
		%pos = getWords(%args, 0, 2);
		%dirt = getWord(%args, 6);
		%isBrick = BTT_isDirtBrick(%dirt);
		if (isObject(%client.BTT_ghostGroup) &&
		    %client.BTT_ghostGroup.isBrick == %isBrick) {
			if (%client.BTT_ghostGroup.position !$= %pos)
				%client.BTT_ghostGroup.setTransform(%pos);
		} else {
			if (isObject(%client.BTT_ghostGroup))
				%client.BTT_ghostGroup.delete();
			%cubeSize = %isBrick ? %client.BTT_cubeSizeBricks : %client.BTT_cubeSizeCubes;
			%newGhost = BTT_ghostGroup(%client, %cubeSize, %pos, %isBrick);
			%client.BTT_ghostGroup = %newGhost;
		}
		if (%isBrick)
			%client.BTT_dirtType = "Brick";
		else
			%client.BTT_dirtType = "Cube";
	}
	%client.BTT_updateText();
	%schedID = schedule(100, 0, BTT_ShovelMode_ghostLoop, %client);
	%client.BTT_shovelMode_schedID = %schedID;
}

function BTT_ShovelMode::fire(%this, %client) {
	if (%client.trenchDirt >= $TrenchDig::dirtCount) {
		%client.centerPrint("\c3You do not have enough room for any more dirt!", 1);
		return;
	}
	%args = BTT_ShovelMode_getGhostPosition(%client);
	if (%args !$= "") {
		// Try to take chunk(s)
		%pos = getWords(%args, 0, 2);
		%normal = getWords(%args, 3, 5);
		%dirt = getWord(%args, 6);
		%isBrick = BTT_isDirtBrick(%dirt);
		%toTake = BTT_chunker(%client);
		if (%isBrick)
			%box = vectorScale("0.5 0.5 0.6", %client.BTT_cubeSizeBricks);
		else
			%box = vectorScale("1 1 1", %client.BTT_cubeSizeCubes);
		%box = vectorSub(%box, "0.1 0.1 0.1");
		%toTake.findChunks(%box, %pos);
		%totalTake = %toTake.getTotalTake();
		if (%client.trenchDirt + %totalTake > $TrenchDig::dirtCount && !%client.isInfiniteMiner) {
			%needed = (%client.trenchDirt + %totalTake) - $TrenchDig::dirtCount;
			%msg = "\c3You do not have enough room for that much dirt!\n" @
				 "\c3You need" SPC %needed SPC "less dirt.";
			%client.centerPrint(%msg, 2);
			// TODO: instead of simply restricting player from digging dirt,
			//       start by digging bricks closest to player
		}
		else {
			%colorIDs = %toTake.take();
			if (!%client.isInfiniteMiner) {
				%start = %client.trenchDirt + 1;
				%client.trenchDirt += %totalTake;
				for (%i = 0; %i < %totalTake; %i++)
					%client.trenchBrick[%start + %i] = getWord(%colorIDs, %i);
			}
		}
		%toTake.delete();
	}
	%client.BTT_updateText();
}

function BTT_ShovelMode::onStartMode(%this, %client) {
	BTT_ShovelMode_ghostLoop(%client);
}

function BTT_ShovelMode::onStopMode(%this, %client) {
	cancel(%client.BTT_shovelMode_schedID);
	if (isObject(%client.BTT_ghostGroup))
		%client.BTT_ghostGroup.delete();
}
