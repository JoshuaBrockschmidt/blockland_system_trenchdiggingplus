////
// Handles preferences
////

function BTT_updateCubeSizes() {
	for (%i = 0; %i < clientGroup.getCount(); %i++) {
		%cl = clientGroup.getObject(%i);
		if (%cl.BTT_cubeSize > $BTT::maxCubeSize) {
			%cl.BTT_cubeSize = $BTT::maxCubeSize;
			if (isObject(%cl.BTT_ghostGroup))
				%cl.BTT_ghostGroup.setSize(%cl.BTT_cubeSize);
		}
	}
}

function BTT_updateColorDefault() {
	// Default dirt color overrides players' choice of dirt color
	if ($BTT::colorIsDefault)
		// TODO: BLG does not yet express manual variable updates in the Glass Server Preferences GUI
		$BTT::canChooseColor = false;
}

if(ForceRequiredAddOn("System_BlocklandGlass") == $Error::AddOn_NotFound) {
	// Maximum size of cubes for placing and digging dirt
	$BTT::maxCubeSize = 4;
	
	// Whether or not players can choose the color of their dirt
	$BTT::canChooseColor = true;

	// Maximum range of tool (both for digging and placing dirt)
	$BTT::toolRange = 10;
}
else {
	registerPref("Better Trench Tool", "Tool", "Max Cube Size", "dropdown", "$BTT::maxCubeSize", "Tool_BetterTrenchTool", 4, "1 1 2 2 3 3 4 4", "BTT_updateCubeSizes", 0, 0, 0);
	registerPref("Better Trench Tool", "Tool", "Tool Range", "dropdown", "$BTT::toolRange", "Tool_BetterTrenchTool", 10, "5 5 10 10 15 15 20 20", "", 0, 0, 0);
	registerPref("Better Trench Tool", "Color", "Players Choose Dirt Color", "bool", "$BTT::canChooseColor", "Tool_BetterTrenchTool", true, "", "", 0, 0, 0);
	registerPref("Better Trench Tool", "Color", "Set Default Dirt Color", "bool", "$BTT::colorIsDefault", "Tool_BetterTrenchTool", false, "BTT_updateColorDefault", "", 0, 0, 0);
	// TODO: check if BLG is compatible with colorset types yet
	registerPref("Better Trench Tool", "Color", "Default Dirt Color", "colorset", "$BTT::defaultColor", "Tool_BetterTrenchTool", 0, "", "", 0, 0, 0);
}
