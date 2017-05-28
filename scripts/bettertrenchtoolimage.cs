////
// Handles equipping and firing of the Better Trench Tool
////

// On tool equipped
function BetterTrenchToolImage::onMount(%this, %player, %slot) {
        %cl = %player.client;
	%cl.BTT_setMode(%cl.BTT_selectedMode);
}

// On tool unequipped
function BetterTrenchToolImage::onUnmount(%this, %player, %slot) {
        %cl = %player.client;
	%cl.BTT_setMode(BTT_DisabledMode);
	%cl.BTT_dirtType = "";
	clearBottomPrint(%cl);
}

function BetterTrenchToolImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, armAttack);
	%obj.schedule(200, playthread, 2, root);
}

function BetterTrenchToolImage::onFire(%this, %player, %slot) {
	%player.client.BTT_mode.fire(%player.client);
}
