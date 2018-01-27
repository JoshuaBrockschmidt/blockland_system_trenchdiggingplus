////
// Provides mode indices and mode function declarations.
////

// TODO: add drag placing
//  * see BuildAndShoot's or Ace of Spades' drag build game mechanic for reference.
//  * should probably be a seperate mode

TDP_ServerGroup.add(
	new ScriptObject(TRT_DisabledMode)
	{
		class = "TRTMode";
		name  = "Disabled Mode";
	});

// See shovelmode.cs and placermode.cs for other modes.

// Updates a client's bottom text for the trench tool, providing information on
// the client's tool mode, cube size, and the quantity of dirt they have.
//
// @param GameConnection this	Target client.
function GameConnection::TRT_updateText(%this) {
	if ($TRT::maxCubeSize > 1)
		%cubeSize = "\c6Cube Size:\c3" SPC %this.TRT_cubeSize;
	else
		%cubeSize = "";

	if (%this.TDP_isInfDirt)
		%dirt = "\c3Infinite Dirt";
	else
		%dirt = "\c3" @ %this.TDP_dirtCnt @ "\c6/\c3" @ $TDP::maxDirt SPC "Dirt";

	if (%this.TDP_isSpeedDirt)
		%modeInfo = "\c6" @ %this.TRT_mode.name SPC "(no delay)";
	else
		%modeInfo = "\c6" @ %this.TRT_mode.name;

	%this.bottomPrint("<just:left>" @ %modeInfo
			  @ "<just:center>" @ %cubeSize
			  @ "            "
			  @ "<just:right>" @ %dirt
			  @ "<br><just:left>\c6(/\c3TTHelp\c6 for help)");
}

// Gets the color ID of a single dirt unit to be placed, as per the trench tool's
// color preferences.
//
// @param GameConnection this	Target client.
// @param int offset		Offset from top of client's dirt stack, if
//				preferences dictate the client's dirt stack be
//				used for color values. 0 by default.
function GameConnection::TRT_getDirtColorID(%this, %offset) {
	if ($TRT::colorIsDefault)
		%colorID = $TDP::defaultColor;
	else if ($TRT::canChooseColor)
		%colorID = %this.currentColor;
	else if (%this.TDP_isInfDirt)
		%colorID = $TDP::defaultColor;
	else
		%colorID = %this.TDP_getDirtColorID(%offset);
	return %colorID;
}

// Gets the color IDs of dirt to be placed by a client, as dictate by dirt
// preferences. Does not subtract from a client's total dirt.
//
// @param GameConnection this	Target client.
// @param int num	        Quantity of dirt to get color IDs from.
function GameConnection::TRT_getDirtColorIDs(%this, %num) {
	for (%i = 0; %i < %num; %i++)
		%colorIDs = %colorIDs SPC %this.TRT_getDirtColorID();
	%colorIDs = trim(%colorIDs);
	return %colorIDs;
}

// Sets the tool mode for the trench tool for a client.
//
// @param GameConnection this	Target client.
// @param TRTMode mode		Mode to switch to.
// @param bool noTextUpdate	Whether or not to update the client's bottom
//			        associated with trench tool. See
//				GameConnection::TRT_updateText() for context.
function GameConnection::TRT_setMode(%this, %mode, %noTextUpdate) {
	// Do not change to mode if that same mode is already active.
	if (%mode.getName() $= %this.TRT_mode.getName())
		return;

	// Switch mode.
	%this.TRT_mode.onStopMode(%this);
	%mode.onStartMode(%this);
	%this.TRT_mode = %mode;

	// Update text.
	if (!%noTextUpdate)
		%this.TRT_updateText();

	// Mount appropriate image.
	%this.TRT_updatingImage = 1;
	%this.player.unmountImage(0);
	%img = %mode.getImage(%this);
	if (isObject(%img))
		%this.player.mountImage(%img, 0);
	%this.TRT_updatingImage = 0;
}

// Default TRTMode functions
function TRTMode::fire(%this, %client){return;}
function TRTMode::onStartMode(%this, %client){return;}
function TRTMode::onStopMode(%this, %client){return;}
function TRTMode::getImage(%this, %client){return -1;}
