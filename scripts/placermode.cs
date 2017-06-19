////
// Mode for placing dirt. Allows you to adjust cube size of dirt placed.
////

// TODO: change speed of placing based on cube size

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
		%dirt = getWord(%args, 6);
		%isBrick = BTT_isDirtBrick(%dirt);
		if (%isBrick) {
			%normal2 = vectorScale(%normal, %client.BTT_cubeSizeBricks);
			%displace = getWord(%normal2, 0) * 0.5
				 SPC getWord(%normal2, 1) * 0.5
				 SPC getWord(%normal2, 2) * 0.6;
		} else {
			%displace = vectorScale(%normal, %client.BTT_cubeSizeCubes);
		}
		%newPos = vectorAdd(%pos, %displace);
		%args2 = %newPos SPC %normal SPC %dirt;
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
			%cubeSize = %isBrick ? %client.BTT_cubeSizeBricks: %client.BTT_cubeSizeCubes;
			%newGhost = BTT_ghostGroup(%client, %cubeSize, %pos, %isBrick);
			%client.BTT_ghostGroup = %newGhost;
		}
		if (%isBrick)
			%client.BTT_dirtType = "Brick";
		else
			%client.BTT_dirtType = "Cube";
	}
	%client.BTT_updateText();
	%schedID = schedule(100, 0, BTT_PlacerMode_ghostLoop, %client);
	%client.BTT_placerMode_schedID = %schedID;
}

function BTT_PlacerMode::fire(%this, %client) {
	// TODO: do not place brick if someone is inside the ghost brick; do not place bricks on players
	//       $TypeMasks::PlayerObjectType
	//       $TypeMasks::VehicleObjectType
	if(%client.trenchDirt <= 0 && !%client.isInfiniteMiner) {
		%client.centerPrint("\c3You have no dirt to release!", 1);
		return;
	}
	%args = BTT_PlacerMode_getGhostPosition(%client);
	if (%args !$= "") {
		%pos = getWords(%args, 0, 2);
		%normal = getWords(%args, 3, 5);
		%dirt = getWord(%args, 6);
		%isBrick = BTT_isDirtBrick(%dirt);
		%brickGroup = %dirt.getGroup();
		%refiller = BTT_refiller(%client, %pos, %isBrick);
		%refiller.planPlacing();
		%numPlace = %refiller.getNumPlace();
		if (%client.trenchDirt < %numPlace && !%client.isInfiniteMiner) {
			%client.centerPrint("\c3You cannot place this much dirt!", 1);
			// TODO: instead of restricting player from placing dirt,
			//       start by placing bricks furthest away as per the %normal
			return;
		}
		else {
			%colorId = %client.currentColor;
			%refiller.place(%client, %colorId, %brickGroup);
			if (!%client.isInfiniteMiner)
				%client.trenchDirt -= %numPlace;
		}
		%refiller.delete();
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
