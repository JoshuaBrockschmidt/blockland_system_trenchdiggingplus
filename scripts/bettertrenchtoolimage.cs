////
// Handles equipping and firing of the Better Trench Tool
////

function BetterTrenchToolImage::onMount(%this, %player, %slot) {
	if (!%player.client.BTT_updatingImage) {
		%player.playThread(2, armReadyRight);
		%cl = %player.client;
		%cl.BTT_setMode(%cl.BTT_selectedMode);
		%player.BTT_updateImage();
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

function BetterTrenchToolImage_1x::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolImage_1x::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolImage_1x::onPreFire(%this, %player, %slot) {
	%player.playThread(2, armAttack);
	%player.schedule(200, playThread, 2, root);
}

function BetterTrenchToolImage_1x::onFire(%this, %player, %slot) {
	%player.client.BTT_mode.fire(%player.client);
}

function BetterTrenchToolImage_2x::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolImage_2x::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolImage_2x::onPreFire(%this, %player, %slot) {
	%player.playThread(2, spearReady);
}

function BetterTrenchToolImage_2x::onFire(%this, %player, %slot) {
	%player.playThread(2, spearThrow);
	%player.client.BTT_mode.fire(%player.client);
}

function BetterTrenchToolImage_3x::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolImage_3x::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolImage_3x::onPreFire(%this, %player, %slot) {
	BetterTrenchToolImage_2x::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolImage_3x::onFire(%this, %player, %slot) {
	BetterTrenchToolImage_2x::onFire(%this, %player, %slot);
}

function BetterTrenchToolImage_4x::onMount(%this, %player, %slot) {
	BetterTrenchToolImage::onMount(%this, %player, %slot);
}

function BetterTrenchToolImage_4x::onUnmount(%this, %player, %slot) {
	BetterTrenchToolImage::onUnmount(%this, %player, %slot);
}

function BetterTrenchToolImage_4x::onPreFire(%this, %player, %slot) {
	BetterTrenchToolImage_2x::onPreFire(%this, %player, %slot);
}

function BetterTrenchToolImage_4x::onFire(%this, %player, %slot) {
	BetterTrenchToolImage_2x::onFire(%this, %player, %slot);
}
