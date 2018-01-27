////
// Includes packaged functions.
////

package TrenchToolPackage {
	function GameConnection::onClientEnterGame(%this) {
		Parent::onClientEnterGame(%this);

		%this.TRT_mode = TRT_DisabledMode;
		%this.TRT_selectedMode = TRT_ShovelMode;
		%this.TRT_cubeSize = 1;
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

	function serverCmdShiftBrick(%cl, %x, %y, %z) {
		// TODO: Make sure ghost brick is shifted as it changes size.
		//       Otherwise, a player may be able to dig while the ghost brick is shifting.
		if (%cl.TRT_mode.getName() !$= TRT_DisabledMode) {
			if (!%cl.TRT_isFiring) {
				if (%z > 0) {
					%cl.TRT_cubeSize++;
					if (%cl.TRT_cubeSize > $TRT::MaxCubeSize)
						%cl.TRT_cubeSize = $TRT::MaxCubeSize;
					else
						%cl.player.playthread(2, shiftUp);
					if (isObject(%cl.TRT_ghostGroup))
						%cl.TRT_ghostGroup.setSize(%cl.TRT_cubeSize);
				} else if (%z < 0) {
					%cl.TRT_cubeSize--;
					if (%cl.TRT_cubeSize < 1)
						%cl.TRT_cubeSize = 1;
					else
						%cl.player.playthread(2, shiftDown);
					if (isObject(%cl.TRT_ghostGroup))
						%cl.TRT_ghostGroup.setSize(%cl.TRT_cubeSize);
				}
				%cl.TRT_updateText();
			}
		}
		else {
			Parent::serverCmdShiftBrick(%cl, %x, %y, %z);
		}
	}

	function serverCmdSuperShiftBrick(%cl, %x, %y, %z) {
		if (%cl.TRT_mode.getName() !$= TRT_DisabledMode) {
			if (!%cl.TRT_isFiring) {
				if (%z > 0) {
					%cl.TRT_cubeSize = $TRT::MaxCubeSize;
					%cl.player.playthread(2, shiftUp);
					%cl.TRT_ghostGroup.setSize(%cl.TRT_cubeSize);
				} else if (%z < 0) {
					%cl.TRT_cubeSize = 1;
					%cl.player.playthread(2, shiftDown);
					%cl.TRT_ghostGroup.setSize(%cl.TRT_cubeSize);
				}
				%cl.TRT_updateText();
			}
		}
		else {
			Parent::serverCmdSuperShiftBrick(%cl, %x, %y, %z);
		}
	}

	function serverCmdLight(%cl) {
		%mode = %cl.TRT_mode.getName();
		if (%mode !$= TRT_DisabledMode) {
			if (%mode $= TRT_ShovelMode) {
				%cl.TRT_setMode(TRT_PlacerMode);
				%cl.TRT_selectedMode = TRT_PlacerMode;
			} else if (%mode $= TRT_PlacerMode) {
				%cl.TRT_setMode(TRT_ShovelMode);
				%cl.TRT_selectedMode = TRT_ShovelMode;
			}
			%cl.TRT_updateText();
		} else {
			Parent::serverCmdLight(%cl);
		}
	}

	function serverCmdDropTool(%cl, %slot) {
		%item = %cl.player.tool[%slot].getName();
		if (%item $= "TrenchToolItem" && %cl.player.currTool == %slot)
			%cl.player.unMountImage(0);

		Parent::serverCmdDropTool(%cl, %slot);
	}

	function GameConnection::TDP_setInfDirt(%this, %bool) {
		Parent::TDP_setInfDirt(%this, %bool);

		if (%this.TRT_mode.getName() !$= TRT_DisabledMode)
			%this.TRT_updateText();
	}

	function GameConnection::TDP_setDirt(%this, %num) {
		Parent::TDP_setDirt(%this, %num);

		if (%this.TRT_mode.getName() !$= TRT_DisabledMode)
			%this.TRT_updateText();
	}

	function GameConnection::TDP_setSpeedDirt(%this, %bool) {
		Parent::TDP_setSpeedDirt(%this, %bool);

		%mode = %this.TRT_mode;
		if (%mode.getName() !$= TRT_DisabledMode) {
			%img = %mode.getImage(%this);
			%this.player.mountImage(%img, 0);
			%this.TRT_updateText();
		}
	}
};
