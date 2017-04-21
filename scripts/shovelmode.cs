////
// Mode for digging dirt. Allows you to adjust cube size of section of dirt to dig.
////

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
	%ray = containerRaycast(%eyePoint, %pos, $TypeMasks::FxBrickObjectType);
	%dirt = firstWord(%ray);
	%normal = normalFromRaycast(%ray);
	if(isObject(%dirt) && %dirt.getDatablock().isTrenchDirt) {
		%rayPos = posFromRaycast(%ray);
		%isBrick = BTT_isDirtBrick(%dirt);
		if (%isBrick) {
			%normalZ = getWord(%normal, 2);
			// If viewing from top.
			if (mFloor(mAbs(%normalZ) + 0.5)) {
				%posXY = getWord(%rayPos, 0)
					SPC getWord(%rayPos, 1)
					SPC 0;
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
					 - 0.3 * (%client.BTT_cubeSizeBricks - 1);
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
			%retArgs = %pos SPC %isBrick;
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
			%retArgs = %onGridPos SPC 0;
		}
	}
	return %retArgs;
}

function BTT_ShovelMode_ghostLoop(%client) {
	%args = BTT_ShovelMode_getGhostPosition(%client);
	%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
	if (trim(%pos) $= "") {
		if (isObject(%client.BTT_ghostGroup))
			%client.BTT_ghostGroup.delete();
		%client.BTT_dirtType = "";
	} else {
		%isBrick = getWord(%args, 3);
		if (isObject(%client.BTT_ghostGroup) &&
		    %client.BTT_ghostGroup.isBrick == %isBrick) {
			if (%client.BTT_ghostGroup.position !$= %pos)
				%client.BTT_ghostGroup.setTransform(%pos);
		} else {
			if (isObject(%client.BTT_ghostGroup))
				%client.BTT_ghostGroup.delete();
			%cubeSize = %isBrick ? %client.BTT_cubeSizeBricks: %client.BTT_cubeSizeCubes;
			%client.BTT_ghostGroup =
				 BTT_ghostGroup(%client,
						%cubeSize,
						%pos,
						%isBrick);
		}
		if (%isBrick)
			%client.BTT_dirtType = "Brick";
		else
			%client.BTT_dirtType = "Cube";
	}
		%client.BTT_updateText();
	%client.BTT_shovelMode_schedID = schedule(100,
						  0,
						  BTT_ShovelMode_ghostLoop,
						  %client);
}

function BTT_ShovelMode::onStartMode(%this, %client) {
	BTT_ShovelMode_ghostLoop(%client);
}

function BTT_ShovelMode::onStopMode(%this, %client) {
	cancel(%client.BTT_shovelMode_schedID);
	if (isObject(%client.BTT_ghostGroup))
		%client.BTT_ghostGroup.delete();
}
