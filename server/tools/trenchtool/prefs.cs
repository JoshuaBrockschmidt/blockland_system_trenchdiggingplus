////
// Handles preferences
////

function TRT_updateCubeSizes() {
	for (%i = 0; %i < clientGroup.getCount(); %i++) {
		%cl = clientGroup.getObject(%i);
		if (%cl.TRT_cubeSize > $TRT::maxCubeSize) {
			%cl.TRT_cubeSize = $TRT::maxCubeSize;
			if (isObject(%cl.TRT_ghostGroup))
				%cl.TRT_ghostGroup.setSize(%cl.TRT_cubeSize);
		}
	}
}

function TRT_updateColorDefault() {
	// Default dirt color overrides players' choice of dirt color
	if ($TRT::colorIsDefault)
		// TODO: BLG does not yet express manual variable updates in the Glass Server Preferences GUI
		$TRT::canChooseColor = false;
}

if(forceRequiredAddOn("System_BlocklandGlass") != $Error::AddOn_NotFound) {
	registerPref("Trench Digging Plus", "Trench Tool", "Max Cube Size", "dropdown", "$TRT::maxCubeSize", "System_TrenchDiggingPlus", 4, "1 1 2 2 3 3 4 4 5 5 6 6", "TRT_updateCubeSizes", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Range", "dropdown", "$TRT::toolRange", "System_TrenchDiggingPlus", 10, "5 5 10 10 15 15 20 20", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Delay Multiplier (seconds)", "num", "$TRT::delayMult", "System_TrenchDiggingPlus", 1, "0 5 1", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Players Choose Dirt Color", "bool", "$TRT::canChooseColor", "System_TrenchDiggingPlus", true, "", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Set Default Dirt Color", "bool", "$TRT::colorIsDefault", "System_TrenchDiggingPlus", false, "TRT_updateColorDefault", "", 0, 0, 0);
	// TODO: check if BLG is compatible with colorset types yet
	registerPref("Trench Digging Plus", "Trench Tool", "Default Dirt Color", "colorset", "$TRT::defaultColor", "System_TrenchDiggingPlus", 0, "", "", 0, 0, 0);
}

// Maximum size of cubes for placing and digging dirt
if ($TRT::maxCubeSize $= "")
	$TRT::maxCubeSize = 4;
	
// Whether or not players can choose the color of their dirt
if ($TRT::canChooseColor $= "")
	$TRT::canChooseColor = true;

// Maximum range of tool (both for digging and placing dirt)
if ($TRT::toolRange $= "")
	$TRT::toolRange = 10;

// Delay multiplier (in seconds) for each increment of cube size
if ($TRT::delayMult $= "")
	$TRT::delayMult = 1;

// Whether or not players can choose their own color
if ($TRT::canChooseColor $= "")
	$TRT::canChooseColor = true;

// Whether or not to use default color
if ($TRT::colorIsDefault $= "")
	$TRT::colorIsDefault = false;

// Default dirt color
if ($TRT::defaultColor $= "")
	$TRT::defaultColor = 0;
