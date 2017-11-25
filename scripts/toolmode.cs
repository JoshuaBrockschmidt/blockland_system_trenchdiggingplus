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

function GameConnection::BTT_updateImage(%this) {
	%size = %this.BTT_cubeSize;
	%playerImg = %this.player.getMountedImage(0).getName();
	if (%this.BTT_mode.index == $BTT::ShovelMode)
		%img = "BetterTrenchToolShovel" @ %size @ "xImage";
	else if (%this.BTT_mode.index == $BTT::PlacerMode)
		%img = "BetterTrenchToolPlacer" @ %size @ "xImage";
	else
		return;

	if (%playerImg !$= "" && %playerImg !$= %img) {
		%this.BTT_updatingImage = 1;
		%this.player.unmountImage();
		%this.player.mountImage(%img);
		%this.BTT_updatingImage = 0;
	}
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
	if (!%noTextUpdate)
		%this.BTT_updateText();
	if (%mode.index != $BTT::DisabledMode)
		%this.BTT_updateImage();
}

function servercmdBTTHelp(%this, %section) {
	%this.chatMessage("\c1==== The Better Trench Tool Guide ====");
	if (%section == 1) {
		%this.chatMessage("\c1---- Description ----");
		%this.chatMessage("\c6The \c4Better Trench Tool \c6is an tool which combines the use of");
		%this.chatMessage("\c6both trench tools, the \c4Trench Shovel \c6and the \c4Trench Dirt\c6,");
		%this.chatMessage("\c6into one, while also providing a few additional features.");
		%this.chatMessage("\c6One feature the \c4Better Trench Tool \c6provides is a ghost brick.");
		%this.chatMessage("\c6This ghost brick is much like the one provided by the standard build");
		%this.chatMessage("\c6tool, in that it places a transparent block where you are looking so");
		%this.chatMessage("\c6you know where you are going to dig/place dirt.");
		%this.chatMessage("\c6Another feature the \c4Better Trench Tool \c6provides is the ability");
		%this.chatMessage("\c6to adjust the amount of dirt you can dig/place at once. Instead of");
		%this.chatMessage("\c6just digging/placing one brick at a time, you choose to dig/place");
		%this.chatMessage("\c6multiple bricks in a single click. This change in your shovel/placer");
		%this.chatMessage("\c6size will be seen in your ghost brick.");
		%this.chatMessage("\c6Some information--like the size of your shovel/place and how much dirt");
		%this.chatMessage("\c6you have--will be present at the bottom of your screen when the");
		%this.chatMessage("\c4Better Trench Tool \c6is equipped.");
		%this.chatMessage("\c6All of this should become more clear when you actually have the");
		%this.chatMessage("\c4Better Trench Tool \c6equipped.");
		%this.chatMessage("\c3(Use Page Up and Page Down to see everything)");
	}
	else if (%section == 2) {
		%this.chatMessage("\c1---- Controls ----");
		%this.chatMessage("\c6Use the <Light Key> \c6to switch between modes.");
		%this.chatMessage("\c3Shovel Mode \c6is for digging trench dirt.");
		%this.chatMessage("\c3Placer Mode \c6is for placing trench dirt.");
		%this.chatMessage("\c6Use <Shift Brick Up> \c6to increase the amount of dirt you place/dig.");
		%this.chatMessage("\c6Use <Shift Brick Down> \c6to decrease the amount of dirt you place/dig.");
		%this.chatMessage("\c6Change your paint color to change the color of the dirt you place.");
	}
	else {
		%this.chatMessage("\c1---- Help Sections ----");
		%this.chatMessage("\c61 - \c3Description");
		%this.chatMessage("\c62 - \c3Controls");
	}
}

// Default BTTMode functions
function BTTMode::fire(%this, %client){return;}
function BTTMode::onStartMode(%this, %client){return;}
function BTTMode::onStopMode(%this, %client){return;}
