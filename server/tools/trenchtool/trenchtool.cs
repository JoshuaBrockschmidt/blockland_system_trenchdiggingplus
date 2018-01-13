package TrenchToolPackage {
	function GameConnection::onClientEnterGame(%this) {
		Parent::onClientEnterGame(%this);

		%this.TRT_mode = TRT_DisabledMode;
		%this.TRT_selectedMode = TRT_ShovelMode;
		%this.TRT_cubeSize = 1;
		if (!%this.trenchDirt)
			%this.trenchDirt = 0;
	}

	function GameConnection::onClientLeaveGame(%this) {
		%this.TRT_setMode(TRT_DisabledMode);
		%this.TRT_ghostGroup.delete();

		Parent::onClientLeaveGame(%this);
	}

	function GameConnection::onDeath(%this, %a, %b, %c, %d) {
		Parent::onDeath(%this, %a, %b, %c, %d);

		%this.TRT_setMode(TRT_DisabledMode, 1);
	}

	function GameConnection::spawnPlayer(%this) {
		Parent::spawnPlayer(%this);

		%this.TRT_setMode(TRT_DisabledMode, 1);
	}

	function GameConnection::updateDirt(%this) {
		if (%this.TRT_mode.getName() $= TRT_DisabledMode)
			Parent::updateDirt(%this);
		else
			%this.TRT_updateText();
	}

	function serverCmdShiftBrick(%client, %x, %y, %z) {
		// TODO: Make sure ghost brick is shifted as it changes size.
		//       Otherwise, a player may be able to dig while the ghost brick is shifting.
		if (%client.TRT_mode.getName() !$= TRT_DisabledMode) {
			if (!%client.TRT_isFiring) {
				if (%z > 0) {
					%client.TRT_cubeSize++;
					if (%client.TRT_cubeSize > $TRT::MaxCubeSize)
						%client.TRT_cubeSize = $TRT::MaxCubeSize;
					else
						%client.player.playthread(2, shiftUp);
					if (isObject(%client.TRT_ghostGroup))
						%client.TRT_ghostGroup.setSize(%client.TRT_cubeSize);
				} else if (%z < 0) {
					%client.TRT_cubeSize--;
					if (%client.TRT_cubeSize < 1)
						%client.TRT_cubeSize = 1;
					else
						%client.player.playthread(2, shiftDown);
					if (isObject(%client.TRT_ghostGroup))
						%client.TRT_ghostGroup.setSize(%client.TRT_cubeSize);
				}
				%client.TRT_updateText();
			}
		}
		else {
			Parent::serverCmdShiftBrick(%client, %x, %y, %z);
		}
	}

	function serverCmdSuperShiftBrick(%client, %x, %y, %z) {
		if (%client.TRT_mode.getName() !$= TRT_DisabledMode) {
			if (!%client.TRT_isFiring) {
				if (%z > 0) {
					%client.TRT_cubeSize = $TRT::MaxCubeSize;
					%client.player.playthread(2, shiftUp);
					%client.TRT_ghostGroup.setSize(%client.TRT_cubeSize);
				} else if (%z < 0) {
					%client.TRT_cubeSize = 1;
					%client.player.playthread(2, shiftDown);
					%client.TRT_ghostGroup.setSize(%client.TRT_cubeSize);
				}
				%client.TRT_updateText();
			}
		}
		else {
			Parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);
		}
	}

	function serverCmdLight(%client) {
		%mode = %client.TRT_mode.getName();
		if (%mode !$= TRT_DisabledMode) {
			if (%mode $= TRT_ShovelMode) {
				%client.TRT_setMode(TRT_PlacerMode);
				%client.TRT_selectedMode = TRT_PlacerMode;
			} else if (%mode $= TRT_PlacerMode) {
				%client.TRT_setMode(TRT_ShovelMode);
				%client.TRT_selectedMode = TRT_ShovelMode;
			}
			%client.TRT_updateText();
		} else {
			Parent::serverCmdLight(%client);
		}
	}

	function serverCmdDropTool(%client, %slot) {
		%item = %client.player.tool[%slot].getName();
		if (%item $= "TrenchToolItem" && %client.player.currTool == %slot)
			%client.player.unMountImage(0);

		Parent::serverCmdDropTool(%client, %slot);
	}
};

activatePackage(TrenchToolPackage);
