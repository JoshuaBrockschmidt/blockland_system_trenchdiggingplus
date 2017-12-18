////
// Handles equipping and firing of the Better Trench Tool
////

function BetterTrenchToolImage::onMount(%this, %player, %slot) {
	%cl = %player.client;
	if (!%cl.BTT_updatingImage) {
		%player.playThread(2, armReadyRight);
		%cl.BTT_setMode(%cl.BTT_selectedMode);
	}
}

function BetterTrenchToolImage::onUnmount(%this, %player, %slot) {
	%cl = %player.client;
	cancel(%this.BTT_preFireSched);
	if (!%cl.BTT_updatingImage) {
		%cl.BTT_setMode(BTT_DisabledMode);
		clearBottomPrint(%cl);
		%player.playThread(2, root);
	}
}

function BetterTrenchToolImage::onReady(%this, %player, %slot) {
	%client.BTT_isFiring = false;
	%player.setImageAmmo(%slot, 1);
}

function BTT_onPreFire_endWait(%player, %slot) {
	%player.setImageAmmo(%slot, 1);
}

function BetterTrenchToolImage::onPreFire(%this, %player, %slot) {
	%cl = %player.client;
	%player.setImageAmmo(%slot, 0);
	%delay = (0.1 + %cl.BTT_cubeSize - 1) * 1000;
	%cl.BTT_preFireSched = schedule(%delay, 0, BTT_onPreFire_endWait, %player, %slot);

	// Handle animation
	if (%cl.BTT_cubeSize == 1) {
		%player.playThread(2, armAttack);
		%player.schedule(200, playThread, 2, root);
	}
	else {
		%player.playThread(2, spearReady);
	}

	%cl.BTT_isFiring = true;
}

function BetterTrenchToolImage::onFire(%this, %player, %slot) {
	%cl = %player.client;
	%emit = %cl.BTT_mode.fire(%player.client);
	if (%emit)
		parent::onFire(%this, %player, %slot);

	// Handle animation
	if (%cl.BTT_cubeSize > 1)
		%player.playThread(2, spearThrow);

	%cl.BTT_isFiring = false;
}


// Shovel mode

function BetterTrenchToolShovelImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolShovelImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolShovelImage::onReady(%this, %player, %slot) {
	BetterTrenchToolImage::onReady(%this, %player, %slot);
}

function BetterTrenchToolShovelImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolShovelImage::onFire(%this, %player, %slot) {
	BetterTrenchToolImage::onFire(%this, %player, %slot);
}


// Placer mode

function BetterTrenchToolPlacerImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolPlacerImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolPlacerImage::onReady(%this, %player, %slot) {
	BetterTrenchToolImage::onReady(%this, %player, %slot);
}

function BetterTrenchToolPlacerImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolPlacerImage::onFire(%this, %player, %slot) {
	BetterTrenchToolImage::onFire(%this, %player, %slot);
}
