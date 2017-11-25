package BetterTrenchToolPackage {
	function GameConnection::onClientEnterGame(%this) {
		parent::onClientEnterGame(%this);

		%this.BTT_mode = BTT_DisabledMode;
		%this.BTT_selectedMode = BTT_ShovelMode;
		%this.BTT_cubeSize = 1;
		if (!%this.trenchDirt)
			%this.trenchDirt = 0;
	}

	function GameConnection::onClientLeaveGame(%this) {
		%this.BTT_setMode(BTT_DisabledMode);
		%this.BTT_ghostGroup.delete();

		parent::onClientLeaveGame(%this);
	}

	function GameConnection::onDeath(%this, %a, %b, %c, %d) {
		parent::onDeath(%this, %a, %b, %c, %d);

		%this.BTT_setMode(BTT_DisabledMode, 1);
	}

	function GameConnection::spawnPlayer(%this) {
		parent::spawnPlayer(%this);

		%this.BTT_setMode(BTT_DisabledMode, 1);
	}

	function GameConnection::updateDirt(%this) {
		if (%this.BTT_mode.index == $BTT::DisabledMode)
			parent::updateDirt(%this);
		else
			%this.BTT_updateText();
	}

	function serverCmdShiftBrick(%client, %x, %y, %z) {
		// TODO: Make sure ghost brick is shifted as it changes size.
		//       Otherwise, a player may be able to dig while the ghost brick is shifting.
		if (%client.BTT_mode.index != $BTT::DisabledMode) {
			if (%z > 0) {
				%client.BTT_cubeSize++;
				if (%client.BTT_cubeSize > $BTT::MaxCubeSize)
					%client.BTT_cubeSize = $BTT::MaxCubeSize;
				else
					%client.player.playthread(2, shiftUp);
				%client.BTT_ghostGroup.setSize(%client.BTT_cubeSize);
			} else if (%z < 0) {
				%client.BTT_cubeSize--;
				if (%client.BTT_cubeSize < 1)
					%client.BTT_cubeSize = 1;
				else
					%client.player.playthread(2, shiftDown);
				%client.BTT_ghostGroup.setSize(%client.BTT_cubeSize);
			}
			%client.BTT_updateText();
			%client.BTT_updateImage();
		}
		else {
			parent::serverCmdShiftBrick(%client, %x, %y, %z);
		}
	}

	function serverCmdSuperShiftBrick(%client, %x, %y, %z) {
		if (%client.BTT_mode.index != $BTT::DisabledMode) {
			if (%z > 0) {
				%client.BTT_cubeSize = $BTT::MaxCubeSize;
				%client.player.playthread(2, shiftUp);
				%client.BTT_ghostGroup.setSize(%client.BTT_cubeSize);
			} else if (%z < 0) {
				%client.BTT_cubeSize = 1;
				%client.player.playthread(2, shiftDown);
				%client.BTT_ghostGroup.setSize(%client.BTT_cubeSize);
			}
			%client.BTT_updateText();
			%client.BTT_updateImage();
		}
		else {
			parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);
		}
	}

	function serverCmdLight(%client) {
		if (%client.BTT_mode.index != $BTT:DisabledMode) {
			if (%client.BTT_mode.index == $BTT::ShovelMode) {
				%client.BTT_setMode(BTT_PlacerMode);
				%client.BTT_selectedMode = BTT_PlacerMode;
			} else if (%client.BTT_mode.index == $BTT::PlacerMode) {
				%client.BTT_setMode(BTT_ShovelMode);
				%client.BTT_selectedMode = BTT_ShovelMode;
			}
			%client.BTT_updateText();
		} else {
			parent::serverCmdLight(%client);
		}
	}

	function serverCmdDropTool(%client, %slot) {
		%item = %client.player.tool[%slot].getName();
		if (%item $= "BetterTrenchToolItem" && %client.player.currTool == %slot)
			%client.player.unMountImage();

		parent::serverCmdDropTool(%client, %slot);
	}
};
