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
			%retArgs = %pos SPC %normal SPC %isBrick SPC %dirt.getGroup();
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
			%retArgs = %onGridPos SPC %normal SPC 0 SPC %dirt.getGroup();
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
		%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
		%isBrick = getWord(%args, 6);
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

function BTT_ShovelMode::fire(%this, %client) {
	// DEBUG
	takeChunk(%client, 1); // TODO: implement function for digging multiple bricks
	return;
	// EOF DEBUG
	// TODO: account for infinite miner
	if(%client.trenchDirt <= 0 && !%client.isInfiniteMiner) {
		%client.centerPrint("\c3You have no dirt to release!",1);
	}
	else {
		%args = BTT_ShovelMode_getGhostPosition(%client);
		if (%args !$= "") {
			%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
			%isBrick = getWord(%args, 6);
			if (%isBrick) {
				%displace = vectorScale("0.25 0.25 0.3", %client.BTT_cubeSizeBricks - 1);
				%cornerPos = vectorSub(%pos, %displace);
				initContainerBoxSearch(%pos, %box, $TypeMasks::fxBrickObjectType);
				%obj = containerSearchNext();
				if(isObject(%obj)) {
					return; // DEBUG
					// TODO: use fxDTSBrickData.brickSizeX (and Y and Z)
				}

				for (%z = 0; %z < %size * 0.6; %z += 0.6) {
					for (%x = 0; %x < %limit8x; %x += 4) {
						for (%y = 0; %y < %limit8x; %y += 4) {
							%brickPos = vectorAdd(%cornerPos, %x SPC %y SPC %z);
							initContainerBoxSearch(%pos, %box, $TypeMasks::fxBrickObjectType);
							%obj = containerSearchNext();
							if(isObject(%obj))
								{
									return %obj;
								}
							else
								{
									return 0;
								}

						}
					}
				}
			}
		}
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
