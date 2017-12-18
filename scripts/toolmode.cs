////
// Provides mode indices and mode function declarations.
////

//TODO: add $BTT:DragPlacer
// see BuildAndShoot's or Ace of Spades' drag build game mechanic for reference.

BTT_ServerGroup.add(
	new ScriptObject(BTT_DisabledMode)
	{
		class = "BTTMode";
		index = $BTT::DisabledMode;
		name  = "Disabled Mode";
	});

// See shovelmode.cs and placermode.cs for other modes.

function GameConnection::BTT_updateText(%this) {
	%cubeSize = "\c6Cube Size:\c3" SPC %this.BTT_cubeSize;

	%this.bottomPrint("<just:left>\c6" @ %this.BTT_mode.name
			  @ "<just:center>" @ %cubeSize
			  @ "            "
			  @ "<just:right>\c3" @ %this.trenchDirt
			  @ "\c6/\c3" @ $TrenchDig::dirtCount
			  SPC "dirt"
			  @ "<br><just:left>\c6(/\c3BTTHelp\c6 for help)");
}

function GameConnection::BTT_getDirtColor(%this, %offset) {
	if ($BTT::colorIsDefault)
		%colorId = $BTT::defaultColor;
	else if ($BTT::canChooseColor)
		%colorId = %this.currentColor;
	else
		%colorId = %this.trenchBrick[%this.trenchDirt - %offset];
}

function GameConnection::BTT_setMode(%this, %mode, %noTextUpdate) {
	if (%mode.index != %this.BTT_mode.index) {
		%this.BTT_mode.onStopMode(%this);
		%mode.onStartMode(%this);
		%this.BTT_mode = %mode;
	}

	// Update text
	if (!%noTextUpdate)
		%this.BTT_updateText();

	// Set image
	%playerImg = %this.player.getMountedImage(0).getName();
	if (%playerImg !$= %mode.image) {
		%this.BTT_updatingImage = 1;
		%this.player.unmountImage(0);
		%this.player.mountImage(%mode.image, 0);
		%this.BTT_updatingImage = 0;
	}
}

function serverCmdBTTHelp(%this, %section) {
	%bullet = "  <font:impact:17>\c9*  <font:palatino linotype:25>";
	%this.chatMessage(" ");
	%this.chatMessage("<rmargin:400><just:center><font:Impact:40><shadow:2:2>\c6Better Trench Tool Help");
	%this.chatMessage("<rmargin:400><just:center><font:Impact:20><shadow:2:2>\c6---------------------------------------------------------------------");
	if (%section == 1) {
		%this.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Description:<br>");
		%this.chatMessage("<rmargin:1000><just:left>");
		%this.chatMessage(%bullet SPC "\c6The \c4Better Trench Tool \c6works as a dirt placer and shovel.");
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

// Default BTTMode functions
function BTTMode::fire(%this, %client){return;}
function BTTMode::onStartMode(%this, %client){return;}
function BTTMode::onStopMode(%this, %client){return;}
