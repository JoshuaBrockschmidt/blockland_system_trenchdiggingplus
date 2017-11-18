////
// Handles preferences
////

function BTT_updateCubeSize() {
	for (%i = 0; %i < clientGroup.getCount(); %i++) {
		%cl = clientGroup.getObject(%i);
		if (%cl.BTT_cubeSizeBricks > $BTT::maxCubeSizeBricks) {
			%cl.BTT_cubeSizeBricks = $BTT::maxCubeSizeBricks;
			if (isObject(%cl.BTT_ghostGroup) && %cl.BTT_ghostGroup.isBrick)
				%cl.BTT_ghostGroup.setSize(%cl.BTT_cubeSizeBricks);
		}
		else if (%cl.BTT_cubeSizeCubes > $BTT::maxCubeSizeCubes) {
			%cl.BTT_cubeSizeCubes = $BTT::maxCubeSizeCubes;
			if (isObject(%cl.BTT_ghostGroup) && !%cl.BTT_ghostGroup.isBrick)
				%cl.BTT_ghostGroup.setSize(%cl.BTT_cubeSizeCubes);
		}
		%cl.BTT_updateText();
	}
}

if(ForceRequiredAddOn("System_BlocklandGlass") == $Error::AddOn_NotFound) {
	// Maximum size of cubes for placing and digging dirt on bricks
	$BTT::maxCubeSizeBricks = 4;

	// Maximum size of cubes for placing and digging dirt on cubes
	$BTT::maxCubeSizeCubes = 4;
	
	// Whether or not players can choose the color of their dirt
	$BTT::canChooseColor = true;

	// Maximum range of tool (both for digging and placing dirt)
	$BTT::toolRange = 10;
}
else {
	registerPref("Better Trench Tool", "General", "Max Cube Size Bricks", "dropdown", "$BTT::maxCubeSizeBricks", "Tool_BetterTrenchTool", 4, "1 1 2 2 3 3 4 4", "BTT_updateCubeSize", 0, 0, 0);
	registerPref("Better Trench Tool", "General", "Max Cube Size Cubes", "dropdown", "$BTT::maxCubeSizeCubes", "Tool_BetterTrenchTool", 4, "1 1 2 2 3 3 4 4", "BTT_updateCubeSize", 0, 0, 0);
	registerPref("Better Trench Tool", "General", "Players Choose Their Dirt Color", "bool", "$BTT::canChooseColor", "Tool_BetterTrenchTool", true, "", "", 0, 0, 0);
	registerPref("Better Trench Tool", "General", "Tool Range", "dropdown", "$BTT::toolRange", "Tool_BetterTrenchTool", 10, "5 5 10 10 15 15 20 20", "", 0, 0, 0);
}
