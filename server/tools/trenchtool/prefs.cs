////
// Handles preferences for the Trench Tool.
////

// TODO: change color preference to a drop-down list

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

function TRT_updateColorPref(%val) {
	switch (%val) {
	case 1: // Actual dirt
		$TRT::canChooseColor = false;
		$TRT::colorIsDefault = false;

	case 2: // Default
		$TRT::canChooseColor = false;
		$TRT::colorIsDefault = true;

	default: // Paint color
		$TRT::canChooseColor = true;
		$TRT::colorIsDefault = false;
	}
}

if (isObject(Glass) && Glass.serverLoaded) {
	registerPref("Trench Digging Plus", "Trench Tool", "Max Cube Size", "dropdown", "TRT::maxCubeSize", "System_TrenchDiggingPlus", 4, "1 1 2 2 3 3 4 4 5 5 6 6", "TRT_updateCubeSizes", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Range", "dropdown", "TRT::toolRange", "System_TrenchDiggingPlus", 10, "5 5 10 10 15 15 20 20", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Delay Multiplier (seconds)", "num", "TRT::delayMult", "System_TrenchDiggingPlus", 1, "0 5 1", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "Trench Tool", "Color Setting", "dropdown", "TRT::colorType", "System_TrenchDiggingPlus", 0, "Paint_Color 0 Actual_Dirt_Color 1 Default_Dirt_Color 2", "TRT_updateColorPref", 0, 0, 0);
}

// Maximum size of cubes for placing and digging dirt
if ($TRT::maxCubeSize $= "")
	$TRT::maxCubeSize = 4;

// Maximum range of tool (both for digging and placing dirt)
if ($TRT::toolRange $= "")
	$TRT::toolRange = 10;

// Delay multiplier (in seconds) for each increment of cube size
if ($TRT::delayMult $= "")
	$TRT::delayMult = 1;

// Whether players can choose the color of their dirt
if ($TRT::canChooseColor $= "")
	$TRT::canChooseColor = true;

// Whether or not to use default color
if ($TRT::colorIsDefault $= "")
	$TRT::colorIsDefault = false;
