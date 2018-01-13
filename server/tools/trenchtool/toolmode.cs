////
// Provides mode indices and mode function declarations.
////

// TODO: add drag placing
//  * see BuildAndShoot's or Ace of Spades' drag build game mechanic for reference.

TDP_ServerGroup.add(
	new ScriptObject(TRT_DisabledMode)
	{
		class = "TRTMode";
		name  = "Disabled Mode";
	});

// See shovelmode.cs and placermode.cs for other modes.

function GameConnection::TRT_updateText(%this) {
	if ($TRT::maxCubeSize > 1)
		%cubeSize = "\c6Cube Size:\c3" SPC %this.TRT_cubeSize;
	else
		%cubeSize = "";

	%this.bottomPrint("<just:left>\c6" @ %this.TRT_mode.name
			  @ "<just:center>" @ %cubeSize
			  @ "            "
			  @ "<just:right>\c3" @ %this.trenchDirt
			  @ "\c6/\c3" @ $TrenchDig::dirtCount
			  SPC "dirt"
			  @ "<br><just:left>\c6(/\c3TTHelp\c6 for help)");
}

function GameConnection::TRT_getDirtColor(%this, %offset) {
	if ($TRT::colorIsDefault)
		%colorId = $TRT::defaultColor;
	else if ($TRT::canChooseColor)
		%colorId = %this.currentColor;
	else
		%colorId = %this.trenchBrick[%this.trenchDirt - %offset];
}

function GameConnection::TRT_setMode(%this, %mode, %noTextUpdate) {
	if (%mode.getName() !$= %this.TRT_mode.getName()) {
		%this.TRT_mode.onStopMode(%this);
		%mode.onStartMode(%this);
		%this.TRT_mode = %mode;
	}

	// Update text
	if (!%noTextUpdate)
		%this.TRT_updateText();

	// Set image
	%playerImg = %this.player.getMountedImage(0).getName();
	if (%playerImg !$= %mode.image) {
		%this.TRT_updatingImage = 1;
		%this.player.unmountImage(0);
		%this.player.mountImage(%mode.image, 0);
		%this.TRT_updatingImage = 0;
	}
}

function serverCmdTTHelp(%this, %section) {
	%bullet = "  <font:impact:17>\c9*  <font:palatino linotype:25>";
	%this.chatMessage(" ");
	%this.chatMessage("<rmargin:400><just:center><font:Impact:40><shadow:2:2>\c6Trench Tool Help");
	%this.chatMessage("<rmargin:400><just:center><font:Impact:20><shadow:2:2>\c6---------------------------------------------------------------------");
	if (%section == 1) {
		%this.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Description:<br>");
		%this.chatMessage("<rmargin:1000><just:left>");
		%this.chatMessage(%bullet SPC "\c6The \c4Trench Tool \c6works as a dirt placer and shovel.");
		%this.chatMessage(%bullet SPC "\c6It also allows you to adjust how much dirt you can dig and place at a time.");
	}
	else if (%section == 2) {
		%this.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Controls:<br>");
		%this.chatMessage("<rmargin:1000><just:left>");
		%this.chatMessage(%bullet SPC "\c6Use the <Light Key> \c6to switch between modes.");
		%this.chatMessage(%bullet SPC "\c3Shovel Mode \c6is for digging trench dirt.");
		%this.chatMessage(%bullet SPC "\c3Placer Mode \c6is for placing trench dirt.");
		%this.chatMessage(%bullet SPC "\c6Use <Shift Brick Up> \c6to increase the amount of dirt you can place and dig.");
		%this.chatMessage(%bullet SPC "\c6Use <Shift Brick Down> \c6to decrease the amount of dirt you can place and dig.");
		%this.chatMessage(%bullet SPC "\c6Use <Super Shift> \c6to quickly change between the smallest and biggest cube size.");
		%this.chatMessage(%bullet SPC "\c6Change your paint color to change the color of the dirt you place.");
		%this.chatMessage("      \c6(if the host has enabled this option)");
	}
	else {
		%this.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Help Sections:<br>");
		%this.chatMessage("<rmargin:1000><just:left>");
		%this.chatMessage("  \c61 - \c3Description");
		%this.chatMessage("  \c62 - \c3Controls");
	}
}

// Default TRTMode functions
function TRTMode::fire(%this, %client){return;}
function TRTMode::onStartMode(%this, %client){return;}
function TRTMode::onStopMode(%this, %client){return;}
