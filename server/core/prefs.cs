////
// Handles preferences
////

if(isObject(Glass) && Glass.serverLoaded) {
	registerPref("Trench Digging Plus", "General", "Max Dirt", "num", "TDP::maxDirt", "System_TrenchDiggingPlus", 500, "1 10000 0", "", 0, 0, 0);
	// TODO: check if BLG is compatible with colorset types yet
	registerPref("Trench Digging Plus", "General", "Default Dirt Color", "colorset", "TDP::defaultColor", "System_TrenchDiggingPlus", 0, "", "", 0, 0, 0);
}

// Maximum quantity of dirt a player can hold
if ($TDP::maxDirt $= "")
	$TDP::maxDirt = 500;

// Default dirt color
if ($TDP::defaultColor $= "")
	$TDP::defaultColor = 0;
