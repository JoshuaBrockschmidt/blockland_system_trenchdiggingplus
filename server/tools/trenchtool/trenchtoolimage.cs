////
// Handles equipping and firing of the Trench Tool.
////

function TrenchToolImage::onMount(%this, %player, %slot) {
	%cl = %player.client;
	if (!%cl.TRT_updatingImage) {
		%player.playThread(2, armReadyRight);
		%cl.TRT_setMode(%cl.TRT_selectedMode);
	}
}

function TrenchToolImage::onUnmount(%this, %player, %slot) {
	%cl = %player.client;
	cancel(%this.TRT_preFireSched);
	if (!%cl.TRT_updatingImage) {
		%cl.TRT_setMode(TRT_DisabledMode);
		clearBottomPrint(%cl);
		%player.playThread(2, root);
	}
}

function TrenchToolImage::onReady(%this, %player, %slot) {
	%client.TRT_isFiring = false;
	%player.setImageAmmo(%slot, 1);
}

function TRT_onPreFire_endWait(%player, %slot) {
	%player.setImageAmmo(%slot, 1);
}

function TrenchToolImage::onPreFire(%this, %player, %slot) {
	%cl = %player.client;
	%player.setImageAmmo(%slot, 0);
	%delay = 100 + (%cl.TRT_cubeSize - 1) * $TRT::delayMult * 1000;
	%cl.TRT_preFireSched = schedule(%delay, 0, TRT_onPreFire_endWait, %player, %slot);

	// Handle animation.
	if (%cl.TRT_cubeSize == 1) {
		%player.playThread(2, armAttack);
		%player.schedule(200, playThread, 2, root);
	}
	else {
		%player.playThread(2, spearReady);
	}

	%cl.TRT_isFiring = true;
}

function TrenchToolImage::onFire(%this, %player, %slot) {
	%cl = %player.client;
	%emit = %cl.TRT_mode.fire(%player.client);
	if (%emit)
		Parent::onFire(%this, %player, %slot);

	// Handle animation
	if (%cl.TRT_cubeSize > 1)
		%player.playThread(2, spearThrow);

	%cl.TRT_isFiring = false;
}


// Shovel mode

function TrenchToolShovelImage::onMount(%this, %player, %slot) {
	TrenchToolImage::onMount(%this, %player, %slot);
}

function TrenchToolShovelImage::onUnmount(%this, %player, %slot) {
	TrenchToolImage::onUnmount(%this, %player, %slot);
}

function TrenchToolShovelImage::onReady(%this, %player, %slot) {
	TrenchToolImage::onReady(%this, %player, %slot);
}

function TrenchToolShovelImage::onPreFire(%this, %player, %slot) {
	TrenchToolImage::onPreFire(%this, %player, %slot);
}

function TrenchToolShovelImage::onFire(%this, %player, %slot) {
	TrenchToolImage::onFire(%this, %player, %slot);
}


// Placer mode

function TrenchToolPlacerImage::onMount(%this, %player, %slot) {
	TrenchToolImage::onMount(%this, %player, %slot);
}

function TrenchToolPlacerImage::onUnmount(%this, %player, %slot) {
	TrenchToolImage::onUnmount(%this, %player, %slot);
}

function TrenchToolPlacerImage::onReady(%this, %player, %slot) {
	TrenchToolImage::onReady(%this, %player, %slot);
}

function TrenchToolPlacerImage::onPreFire(%this, %player, %slot) {
	TrenchToolImage::onPreFire(%this, %player, %slot);
}

function TrenchToolPlacerImage::onFire(%this, %player, %slot) {
	TrenchToolImage::onFire(%this, %player, %slot);
}


// Speed shovel mode

function TrenchToolSpeedShovelImage::onMount(%this, %player, %slot) {
	TrenchToolImage::onMount(%this, %player, %slot);
}

function TrenchToolSpeedShovelImage::onUnmount(%this, %player, %slot) {
	TrenchToolImage::onUnmount(%this, %player, %slot);
}

function TrenchToolSpeedShovelImage::onReady(%this, %player, %slot) {
	%player.playThread(2, root);
}

function TrenchToolSpeedShovelImage::onPreFire(%this, %player, %slot) {
	%player.playThread(2, armAttack);
}

function TrenchToolSpeedShovelImage::onFire(%this, %player, %slot) {
	TrenchToolImage::onFire(%this, %player, %slot);
}


// Speed placer mode

function TrenchToolSpeedPlacerImage::onMount(%this, %player, %slot) {
	TrenchToolImage::onMount(%this, %player, %slot);
}

function TrenchToolSpeedPlacerImage::onUnmount(%this, %player, %slot) {
	TrenchToolImage::onUnmount(%this, %player, %slot);
}

function TrenchToolSpeedPlacerImage::onReady(%this, %player, %slot) {
        TrenchToolSpeedShovelImage::onReady(%this, %player, %slot);
}

function TrenchToolSpeedPlacerImage::onPreFire(%this, %player, %slot) {
	TrenchToolSpeedShovelImage::onPreFire(%this, %player, %slot);
}

function TrenchToolSpeedPlacerImage::onFire(%this, %player, %slot) {
	TrenchToolImage::onFire(%this, %player, %slot);
}
