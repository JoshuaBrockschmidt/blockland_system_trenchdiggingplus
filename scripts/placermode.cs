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
	%eyePoint = %client.player.getEyePoint();
	%eyeVector = %client.player.getEyeVector();
	%pos = vectorAdd(%eyePoint, vectorscale(%eyeVector, $BTT::toolRange));
	%ray = containerRaycast(%eyePoint, %pos, $TypeMasks::FxBrickObjectType);
	%normal = normalFromRaycast(%ray);
	//%dirt = firstWord(%ray);
	%args = BTT_ShovelMode_getGhostPosition(%client);
	%pos = getWord(%args, 0) SPC getWord(%args, 1) SPC getWord(%args, 2);
	if (%args !$= "") {
		%isBrick = getWord(%args, 3);
		if (%isBrick) {
			%normal2 = vectorScale(%normal, %client.BTT_cubeSizeBricks);
			%displace = getWord(%normal2, 0) * 0.5
				 SPC getWord(%normal2, 1) * 0.5
				 SPC getWord(%normal2, 2) * 0.6;
		} else {
			%displace = vectorScale(%normal, %client.BTT_cubeSizeCubes);
		}
		%newPos = vectorAdd(%pos, %displace);
		%args2 = %newPos SPC %isBrick;
	}
	return %args2;
}

function BTT_PlacerMode_ghostLoop(%client) {
	%args = BTT_PlacerMode_getGhostPosition(%client);
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
	%client.BTT_placerMode_schedID = schedule(100,
						  0,
						  BTT_PlacerMode_ghostLoop,
						  %client);
}

function BTT_PlacerMode::onStartMode(%this, %client) {
	BTT_PlacerMode_ghostLoop(%client);
}

function BTT_PlacerMode::onStopMode(%this, %client) {
	cancel(%client.BTT_placerMode_schedID);
	if (isObject(%client.BTT_ghostGroup))
		%client.BTT_ghostGroup.delete();
}
