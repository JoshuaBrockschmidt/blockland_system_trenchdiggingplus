////
// Mode for placing dirt. Allows you to adjust cube size of dirt placed.
////

BTT_ServerGroup.add(
	new ScriptObject(BTT_PlacerMode)
	{
		class = "BTTMode";
		name  = "Placer Mode";
		image = BetterTrenchToolPlacerImage;
	});

function BTT_PlacerMode_getGhostPosition(%client) {
	%args = BTT_ShovelMode_getGhostPosition(%client);
	if (%args !$= "") {
		%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
		%normal = getWord(%args, 3) SPC getWord(%args, 4) SPC getWord(%args, 5);
		%dirt = getWord(%args, 6);
		%isBrick = BTT_isDirtBrick(%dirt);
		%displace = vectorScale(%normal, %client.BTT_cubeSize);
		if (%isBrick) {
			%displace = getWord(%displace, 0) * 0.5
				 SPC getWord(%displace, 1) * 0.5
				 SPC getWord(%displace, 2) * 0.6;
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
			%newGhost = BTT_ghostGroup(%client, %client.BTT_cubeSize, %pos, %isBrick);
			%client.BTT_ghostGroup = %newGhost;
		}
	}
	if (isObject(%client.BTT_ghostGroup))
	        %client.BTT_ghostGroup.updateColor();
	%client.BTT_updateText();
	%schedID = schedule(100, 0, BTT_PlacerMode_ghostLoop, %client);
	%client.BTT_placerMode_schedID = %schedID;
}

function BTT_PlacerMode::fire(%this, %client) {
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

		// Check if there is a vehicle or player object in the way.
		if (%isBrick)
			%box = vectorScale("0.5 0.5 0.6", %client.BTT_cubeSize);
		else
			%box = vectorScale("1 1 1", %client.BTT_cubeSize);
	    %box = vectorSub(%box, "0.1 0.1 0.1");
		%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
		%aabb = vectorSub(%pos, vectorScale(%box, 0.5)) SPC %box;
		initContainerBoxSearch(%pos, %box, %mask);
		while (isObject(%obj = containerSearchNext())) {
			// We will only check if a player/vehicle object's
			// position vector is within the player's cube
			// selection, since the bounding box of the
			// player/vehicle object detected by the container
			// search extends past the object's actual collision
			// box.
			%objPos = vectorAdd(%obj.position, "0 0 0.1");
			if (BTT_PointAABB_3D(%objPos, %aabb)) {
				%msg = "\c3You cannot place that here!\n" @
					"\c3There is something in the way.";
				%client.centerPrint(%msg, 2);
				return;
			}
		}

		// Attempt to place dirt.
		%refiller = BTT_refiller(%client, %pos, %isBrick);
		%refiller.planPlacing();
		%numPlace = %refiller.getNumPlace();
		if (%client.trenchDirt < %numPlace && !%client.isInfiniteMiner) {
			%needed = %numPlace - %client.trenchDirt;
			%msg = "\c3You cannot place this much dirt!\n" @
				 "\c3You need" SPC %needed SPC "more dirt.";
			%client.centerPrint(%msg, 2);
			// TODO: instead of restricting player from placing dirt,
			//       start by placing bricks furthest away as per the %normal
		}
		else {
			%brickGroup = %dirt.getGroup();
			%refiller.place(%client, %brickGroup);
			if (!%client.isInfiniteMiner)
				%client.trenchDirt -= %numPlace;
		}
		%refiller.delete();
	}
	%client.BTT_updateText();
	if (%numplace > 0)
		return 1;
	else
		return 0;
}

function BTT_PlacerMode::onStartMode(%this, %client) {
	BTT_PlacerMode_ghostLoop(%client);
}

function BTT_PlacerMode::onStopMode(%this, %client) {
	cancel(%client.BTT_placerMode_schedID);
	if (isObject(%client.BTT_ghostGroup))
		%client.BTT_ghostGroup.delete();
}
