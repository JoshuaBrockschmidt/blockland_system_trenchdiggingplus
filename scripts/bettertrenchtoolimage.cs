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
	if (%player.client.BTT_mode.index == $BTT::ShovelMode) {
		takeChunk(%player.client, 1); // TODO: implement function for digging multiple bricks
		%player.client.BTT_updateText();
	} else if (%player.client.BTT_mode.index == $BTT::PlacerMode) {
		shootChunk(%player.client); // TODO: implement function for placing multiple bricks
		%player.client.BTT_updateText();
	}
}
