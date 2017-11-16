////
// Handles equipping and firing of the Better Trench Tool
////

function BetterTrenchToolImage::onMount(%this, %player, %slot) {
	if (!%player.client.BTT_updatingImage) {
		%player.playThread(2, armReadyRight);
		%cl = %player.client;
		%cl.BTT_setMode(%cl.BTT_selectedMode);
		%cl.BTT_updateImage();
	}
}

function BetterTrenchToolImage::onUnmount(%this, %player, %slot) {
	if (!%player.client.BTT_updatingImage) {
		%cl = %player.client;
		%cl.BTT_setMode(BTT_DisabledMode);
		%cl.BTT_dirtType = "";
		clearBottomPrint(%cl);
		%player.playThread(2, root);
	}
}


// Shovel mode

function BetterTrenchToolShovel1xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolShovel1xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolShovel1xImage::onPreFire(%this, %player, %slot) {
	%player.playThread(2, armAttack);
	%player.schedule(200, playThread, 2, root);
}

function BetterTrenchToolShovel1xImage::onFire(%this, %player, %slot) {
	%emit = %player.client.BTT_mode.fire(%player.client);
	if (%emit)
		Parent::onFire(%this, %player, %slot);
}

function BetterTrenchToolShovel2xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolShovel2xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolShovel2xImage::onPreFire(%this, %player, %slot) {
	%player.playThread(2, spearReady);
}

function BetterTrenchToolShovel2xImage::onFire(%this, %player, %slot) {
	%player.playThread(2, spearThrow);
	BetterTrenchToolShovel1xImage::onFire(%this, %player, %slot);
}

function BetterTrenchToolShovel3xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolShovel3xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolShovel3xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolShovel3xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onFire(%this, %player, %slot);
}

function BetterTrenchToolShovel4xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolShovel4xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolShovel4xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolShovel4xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onFire(%this, %player, %slot);
}


// Placer mode

function BetterTrenchToolPlacer1xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolShovel1xImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolPlacer1xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolShovel1xImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolPlacer1xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel1xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer1xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel1xImage::onFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer2xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolPlacer2xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolPlacer2xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer2xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel2xImage::onFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer3xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolShovel3xImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolPlacer3xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolShovel3xImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolPlacer3xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel3xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer3xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel3xImage::onFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer4xImage::onMount(%this, %player, %slot) {
	BetterTrenchToolShovel4xImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolPlacer4xImage::onUnmount(%this, %player, %slot) {
	BetterTrenchToolShovel4xImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolPlacer4xImage::onPreFire(%this, %player, %slot) {
	BetterTrenchToolShovel4xImage::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolPlacer4xImage::onFire(%this, %player, %slot) {
	BetterTrenchToolShovel4xImage::onFire(%this, %player, %slot);
}
