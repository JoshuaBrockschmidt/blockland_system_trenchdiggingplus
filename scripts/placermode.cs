////
// Mode for placing dirt. Allows you to adjust cube size of dirt placed.
////

BTT_ServerGroup.add(
	new ScriptObject(BTT_PlacerMode)
	{
		class = "BTTMode";
		index = $BTT::PlacerMode;
		name  = "Placer Mode";
	});

function BTT_PlacerMode_getGhostPosition(%client) {
	%args = BTT_ShovelMode_getGhostPosition(%client);
	if (%args !$= "") {
		%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
		%normal = getWord(%args, 3) SPC getWord(%args, 4) SPC getWord(%args, 5);
		%isBrick = getWord(%args, 6);
		if (%isBrick) {
			%normal2 = vectorScale(%normal, %client.BTT_cubeSizeBricks);
			%displace = getWord(%normal2, 0) * 0.5
				 SPC getWord(%normal2, 1) * 0.5
				 SPC getWord(%normal2, 2) * 0.6;
		} else {
			%displace = vectorScale(%normal, %client.BTT_cubeSizeCubes);
		}
		%newPos = vectorAdd(%pos, %displace);
		%args2 = %newPos SPC %normal SPC %isBrick SPC getWord(%args, 7);
	}
	return %args2;
}

function BTT_PlacerMode_ghostLoop(%client) {
	%args = BTT_PlacerMode_getGhostPosition(%client);
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
	%client.BTT_placerMode_schedID = schedule(100,
						  0,
						  BTT_PlacerMode_ghostLoop,
						  %client);
}

function BTT_PlacerMode::fire(%this, %client) {
	if(%client.trenchDirt <= 0 && !%client.isInfiniteMiner) {
		%client.centerPrint("\c3You have no dirt to release!", 1);
	}
	else {
		%args = BTT_PlacerMode_getGhostPosition(%client);
		if (%args !$= "") {
			%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
			%normal = getWord(%args, 3) SPC getWord(%args, 4) SPC getWord(%args, 5);
			%isBrick = getWord(%args, 6);
			%brickGroup = getWord(%args, 7);
			if (%isBrick) {
				%displace = vectorScale("0.25 0.25 0.3", %client.BTT_cubeSizeBricks - 1);
				%cornerPos = vectorSub(%pos, %displace);
				%numBricks = 0;
				%cont = 1;
				// TODO: get normal and begin by placing bricks furthest away
				for (%z = 0; %z < %client.BTT_cubeSizeBricks * 0.6; %z += 0.6) {
					for (%x = 0; %x < %client.BTT_cubeSizeBricks * 0.5; %x += 0.5) {
						for (%y = 0; %y < %client.BTT_cubeSizeBricks * 0.5; %y += 0.5) {
							%brickPos = vectorAdd(%cornerPos, %x SPC %y SPC %z);
							%newBrick = new fxDtsBrick() {
								position = %brickPos;
								datablock = brick1x1DirtData;
								colorId = %client.currentColor;
								colorFxId = 0;
								shapeFxId = 0;
								client = %client;
							};
							%newBrick.isPlanted = 1;
							%newBrick.setTrusted(1);
							%error = %newBrick.plant();
							if(%error && %error != 2) {
								%newBrick.delete();
							} else {
								%brickGroup.add(%newBrick);
								%bricks[%numBricks] = %newBrick;
								%numBricks++;
								%client.trenchDirt--;
								if (%client.trenchDirt == 0) {
									%cont = 0;
									break;
								}
							}
						}
						if (!%cont) break;
					}
					if (!%cont) break;
				}
				for (%i = 0; %i < %numBricks; %i++) {
					%b = %bricks[%i];
					if (isObject(%b))
						BTT_refill(%b);
				}
			}
		}
	}
	%client.BTT_updateText();
}

function BTT_PlacerMode::onStartMode(%this, %client) {
	BTT_PlacerMode_ghostLoop(%client);
}

function BTT_PlacerMode::onStopMode(%this, %client) {
	cancel(%client.BTT_placerMode_schedID);
	if (isObject(%client.BTT_ghostGroup))
		%client.BTT_ghostGroup.delete();
}
