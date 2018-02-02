////
// Mode for placing dirt. Allows you to adjust cube size of dirt placed.
////

TDP_ServerGroup.add(
	new ScriptObject(TRT_PlacerMode)
	{
		class = "TRTMode";
		name  = "Placer Mode";
	});

function TRT_PlacerMode_getGhostPosition(%client) {
	%args = TRT_ShovelMode_getGhostPosition(%client);
	if (%args !$= "") {
		%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
		%normal = getWord(%args, 3) SPC getWord(%args, 4) SPC getWord(%args, 5);
		%dirt = getWord(%args, 6);
		%isBrick = TDP_isDirtBrick(%dirt);
		%displace = vectorScale(%normal, %client.TRT_cubeSize);
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

function TRT_PlacerMode_ghostLoop(%client) {
	%args = TRT_PlacerMode_getGhostPosition(%client);
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
	%schedID = schedule(100, 0, TRT_PlacerMode_ghostLoop, %client);
	%client.TRT_placerMode_schedID = %schedID;
}

function TRT_PlacerMode::fire(%this, %client) {
	if(%client.TDP_dirtCnt <= 0 && !(%client.TDP_isInfDirt || $TDP::infDirtForAll)) {
		%client.centerPrint("\c3You have no dirt to release!", 1);
		return;
	}
	%args = TRT_PlacerMode_getGhostPosition(%client);
	if (%args !$= "") {
		%pos = getWords(%args, 0, 2);
		%normal = getWords(%args, 3, 5);
		%dirt = getWord(%args, 6);
		%isBrick = TDP_isDirtBrick(%dirt);

		// Check if there is a vehicle or player object in the way.
		if (%isBrick)
			%box = vectorScale("0.5 0.5 0.6", %client.TRT_cubeSize);
		else
			%box = vectorScale("1 1 1", %client.TRT_cubeSize);
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
			if (TDP_pointAABB_3D(%objPos, %aabb)) {
				%msg = "\c3You cannot place that here!\n" @
					"\c3There is something in the way.";
				%client.centerPrint(%msg, 2);
				return;
			}
		}		

		// Attempt to place dirt.
		%refiller = TDP_Refiller(%pos, %client.TRT_cubeSize, %isBrick);
		%refiller.planPlacing();
		%numPlace = %refiller.getNumPlace();
		if (%client.TDP_dirtCnt < %numPlace && !(%client.TDP_isInfDirt || $TDP::infDirtForAll)) {
			%needed = %numPlace - %client.TDP_dirtCnt;
			%msg = "\c3You cannot place this much dirt!\n" @
				 "\c3You need" SPC %needed SPC "more dirt.";
			%client.centerPrint(%msg, 2);
			// TODO: instead of restricting player from placing dirt,
			//       start by placing bricks furthest away as per the %normal
		}
		else {
			%colorIDs = %client.TRT_getDirtColorIDs(%numPlace);
			%brickGroup = %dirt.getGroup();
			%refiller.place(%dirt.client, %brickGroup, %client, %colorIDs);
			if (!(%client.TDP_isInfDirt || $TDP::infDirtForAll))
				%colorIDs = %client.TDP_subDirt(%numPlace);
	        }
		%refiller.delete();
	}
	%client.TRT_updateText();
	if (%numplace > 0)
		return 1;
	else
		return 0;
}

function TRT_PlacerMode::onStartMode(%this, %client) {
	TRT_PlacerMode_ghostLoop(%client);
}

function TRT_PlacerMode::onStopMode(%this, %client) {
	cancel(%client.TRT_placerMode_schedID);
	if (isObject(%client.TRT_ghostGroup))
		%client.TRT_ghostGroup.delete();
}

function TRT_PlacerMode::getImage(%this, %client) {
	if (%client.TDP_isSpeedDirt)
		return TrenchToolSpeedPlacerImage;
	else
		return TrenchToolPlacerImage;
}
