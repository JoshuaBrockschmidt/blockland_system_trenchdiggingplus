////
// Assorted functions that do not belong in any one file.
////

function vectorFloor(%vec) {
	%newVec = mFloor(getWord(%vec, 0))
		SPC mFloor(getWord(%vec, 1))
		SPC mFloor(getWord(%vec, 2));
	return %newVec;
}

function BTT_isDirtCube(%brick) {
	%dbName = %brick.getDatablock().getName();
	if (%dbName $= "brick2xCubeDirtData"  ||
	    %dbName $= "brick4xCubeDirtData"  ||
	    %dbName $= "brick8xCubeDirtData"  ||
	    %dbName $= "brick16xCubeDirtData" ||
	    %dbName $= "brick32xCubeDirtData" ||
	    %dbName $= "brick64xCubeDirtData") {
		return 1;
	} else {
		return 0;
	}
}

function BTT_isDirtBrick(%brick) {
	%dbName = %brick.getDatablock().getName();
	if (%dbName $= "brick8x16DirtData" ||
	    %dbName $= "brick8x8DirtData"  ||
	    %dbName $= "brick4x4DirtData"  ||
	    %dbName $= "brick2x2DirtData"  ||
	    %dbName $= "brick1x1DirtData") {
		return 1;
	} else {
		return 0;
	}
}

function BTT_refill_findBrick(%pos, %box, %dbName) {
	initContainerBoxSearch(%pos, %box, $TypeMasks::fxBrickObjectType);
	%brick = containerSearchNext();
	while (isObject(%brick)) {
		if (!%brick.isPlanted)
			%brick = containerSearchNext();
		else if (isObject(%brick) &&
			 %brick.getDatablock().getName() $= %dbName &&
			 %brick.position $= %pos)
			return %brick;
		else 
			return;
	}
}

// Returns the most prevelant color among a vector of dirt bricks.
// In the case that no one color is most prevalent, the color of the
// first dirt brick in the array will be returned.
function BTT_refill_getColor(%bricks) {
	%numBricks = getWordCount(%bricks);
	%numColors = 0;
	// Tally the colors
	for (%b = 0; %b < %numBricks; %b++) {
		%thisColor = getWord(%bricks, %b).colorId;
		%colorFound = 0;
		for (%c = 0; %c < %numColors; %c++) {
			if (%colors[%c] == %thisColor) {
				%colorFound = 1;
				%colorTally[%c]++;
				break;
			}
		}
		if (!%colorFound) {
			%colors[%numColors] = %thisColor;
			%colorTally[%numColors] = 1;
			%numColors++;
		}
	}
	// Find the most prevalent color.
	%maxColor = %colors[0];
	%maxTally = %colorTally[0];
	for (%c = 1; %c < %numColors; %c++) {
		if (%colorTally[%c] > %maxTally) {
			%maxColor = %colors[%c];
			%maxTally = %colorTally[%c];
		}
	}
	return %maxColor;
}

function BTT_refill_replace(%bricks, %newPos) {
	%dbName = getWord(%bricks, 0).getDatablock().getName();
	%doRefill = 1;
	%rot = "1 0 0 0";
	if (%dbName $= "brick1x1DirtData") {
		%newDbName = "brick2x2DirtData";
	}
	else if (%dbName $= "brick2x2DirtData") {
		%newDbName = "brick4x4DirtData";
	}
	else if (%dbName $= "brick4x4DirtData") {
		%newDbName = "brick8x8DirtData";
	}
	else if (%dbName $= "brick8x8DirtData") {
		%newDbName = "brick8x16DirtData";
		%diff = vectorSub(getWord(%bricks, 0).position, getWord(%bricks, 1).position);
		if (getWord(%diff, 0) != 0)
			%rot = "0 0 1 90";
		%doRefill = 0;
	}
	else if (%dbName $= "brick2xCubeDirtData") {
		%newDbName = "brick4xCubeDirtData";
	}
	else if (%dbName $= "brick4xCubeDirtData") {
		%newDbName = "brick8xCubeDirtData";
	}
	else if (%dbName $= "brick8xCubeDirtData") {
		%newDbName = "brick16xCubeDirtData";
	}
	else if (%dbName $= "brick16xCubeDirtData") {
		%newDbName = "brick32xCubeDirtData";
	}
	else if (%dbName $= "brick32xCubeDirtData") {
		%newDbName = "brick64xCubeDirtData";
		%doRefill = 0;
	}
	else {
		return;
	}
	%brick1 = getWord(%bricks, 0);
	%brickGroup = %brick1.getGroup();
	%colorId = BTT_refill_getColor(%bricks);
	%brickNum = getWordCount(%bricks);
	for (%b = 0; %b < %brickNum; %b++)
		getWord(%bricks, %b).delete();
	%newDirt = new fxDTSBrick() {
		client = %brick1.client;
		colorFxId = 0;
		colorId = %colorId;
		datablock = %newDbName;
		position = %newPos;
		rotation = %rot;
		shapeFxId = 0;
	};
	%newDirt.isPlanted = 1;
	%newDirt.setTrusted(1);
	%error = %newDirt.plant();
	if(%error && %error != 2)
		%newDirt.delete();
	else
		%brickGroup.add(%newDirt);
	if (%doRefill)
		BTT_refill(%newDirt);
}

function BTT_refill_brick_small(%brick) {
	%dbName = %brick.getDatablock().getName();
	%size = %dbName.brickSizeX;
	%pos = %brick.position;
	%box = 0.5 * %size - 0.1
		 SPC 0.5 * %size - 0.1
		 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = BTT_refill_findBrick(vectorAdd(%pos, %northVec), %box, %dbName);
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = BTT_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %dbName);
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = BTT_refill_findBrick(vectorAdd(%pos, %westVec), %box, %dbName);
	if (isObject(%north)) {
		// northeast
		if (isObject(%east)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne = BTT_refill_findBrick(vectorAdd(%pos, %neVec), %box, %dbName);
			if (isObject(%ne)) {
				%bricks = %brick SPC %north SPC %ne SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 0.25 0", %size));
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// northwest
		if (isObject(%west)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw = BTT_refill_findBrick(vectorAdd(%pos, %nwVec), %box, %dbName);
			if (isObject(%nw)) {
				%bricks = %brick SPC %north SPC %nw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 0.25 0", %size));
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = BTT_refill_findBrick(vectorAdd(%pos, %southVec), %box, %dbName);
	if (isObject(%south)) {
		// southeast
		if (isObject(%east)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se = BTT_refill_findBrick(vectorAdd(%pos, %seVec), %box, %dbName);
			if (isObject(%se)) {
				%bricks = %brick SPC %south SPC %se SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 -0.25 0", %size));
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// southwest
		if (isObject(%west)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw = BTT_refill_findBrick(vectorAdd(%pos, %swVec), %box, %dbName);
			if (isObject(%sw)) {
				%bricks = %brick SPC %south SPC %sw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 -0.25 0", %size));
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
}

function BTT_refill_brick_8x8(%brick) {
	%dbName = %brick.getDatablock().getName();
	%size = %dbName.brickSizeX;
	%pos = %brick.position;
	%box = 3.9 SPC 3.9 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = BTT_refill_findBrick(vectorAdd(%pos, %northVec), %box, %dbName);
	if (isObject(%north)) {
		%bricks = %brick SPC %north;
		%newPos = vectorAdd(%pos, "0 2 0");
		BTT_refill_replace(%bricks, %newPos);
		return;
	}
	// east
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = BTT_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %dbName);
	if (isObject(%east)) {
		%bricks = %brick SPC %east;
		%newPos = vectorAdd(%pos, "2 0 0");
		BTT_refill_replace(%bricks, %newPos);
		return;
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = BTT_refill_findBrick(vectorAdd(%pos, %southVec), %box, %dbName);
	if (isObject(%south)) {
		%bricks = %brick SPC %south;
		%newPos = vectorAdd(%pos, "0 -2 0");
		BTT_refill_replace(%bricks, %newPos);
		return;
	}
	// west
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = BTT_refill_findBrick(vectorAdd(%pos, %westVec), %box, %dbName);
	if (isObject(%west)) {
		%bricks = %brick SPC %west;
		%newPos = vectorAdd(%pos, "-2 0 0");
		BTT_refill_replace(%bricks, %newPos);
		return;
	}
}

function BTT_refill_cube_sect(%baseBrick, %brick2, %box) {
	%db = %baseBrick.getDatablock();
	%dbName = %db.getName();
	%pos1 = %baseBrick.position;
	%pos2 = %brick2.position;
	%northVec = vectorScale("0 0.5 0", %db.brickSizeX);
	%north1 = BTT_refill_findBrick(vectorAdd(%pos1, %northVec), %box, %dbName);
	%north2 = BTT_refill_findBrick(vectorAdd(%pos2, %northVec), %box, %dbName);
	%eastVec = vectorScale("0.5 0 0", %db.brickSizeX);
	%east1 = BTT_refill_findBrick(vectorAdd(%pos1, %eastVec), %box, %dbName);
	%east2 = BTT_refill_findBrick(vectorAdd(%pos2, %eastVec), %box, %dbName);
	%westVec = vectorScale("-0.5 0 0", %db.brickSizeX);
	%west1 = BTT_refill_findBrick(vectorAdd(%pos1, %westVec), %box, %dbName);
	%west2 = BTT_refill_findBrick(vectorAdd(%pos2, %westVec), %box, %dbName);
	// north
	if (isObject(%north1) && isObject(%north2)) {
		// northeast
		if (isObject(%east1) && isObject(%east2)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne1 = BTT_refill_findBrick(vectorAdd(%pos1, %neVec), %box, %dbName);
			%ne2 = BTT_refill_findBrick(vectorAdd(%pos2, %neVec), %box, %dbName);
			if (isObject(%ne1) && isObject(%ne2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					%north1 SPC %north2 SPC
					%ne1 SPC %ne2 SPC
				        %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%neVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// northwest
		if (isObject(%west1) && isObject(%west2)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw1 = BTT_refill_findBrick(vectorAdd(%pos1, %nwVec), %box, %dbName);
			%nw2 = BTT_refill_findBrick(vectorAdd(%pos2, %nwVec), %box, %dbName);
			if (isObject(%nw1) && isObject(%nw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
				        %north1 SPC %north2 SPC
					%nw1 SPC %nw2 SPC
					%west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%nwVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %db.brickSizeX);
	%south1 = BTT_refill_findBrick(vectorAdd(%pos1, %southVec), %box, %dbName);
	%south2 = BTT_refill_findBrick(vectorAdd(%pos2, %southVec), %box, %dbName);
	if (isObject(%south1) && isObject(%south2)) {
		// southeast
		if (isObject(%east1) && isObject(%east2)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se1 = BTT_refill_findBrick(vectorAdd(%pos1, %seVec), %box, %dbName);
			%se2 = BTT_refill_findBrick(vectorAdd(%pos2, %seVec), %box, %dbName);
			if (isObject(%se1) && isObject(%se2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %se1 SPC %se2 SPC
					 %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%seVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// southwest
		if (isObject(%west1) && isObject(%west2)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw1 = BTT_refill_findBrick(vectorAdd(%pos1, %swVec), %box, %dbName);
			%sw2 = BTT_refill_findBrick(vectorAdd(%pos2, %swVec), %box, %dbName);
			if (isObject(%sw1) && isObject(%sw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %sw1 SPC %sw2 SPC
					 %west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%swVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				BTT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
}

function BTT_refill_cube(%brick) {
	%db = %brick.getDatablock();
	%pos = %brick.position;
	%box = %db.brickSizeX * 0.5 SPC %db.brickSizeY * 0.5 SPC %db.brickSizeZ * 0.2;
	%box = vectorSub(%box, "0.1 0.1 0.1");
	%upVec = "0 0" SPC %db.brickSizeZ * 0.2;
	%up = BTT_refill_findBrick(vectorAdd(%pos, %upVec), %box, %db.getName());
	if (isObject(%up)) {
		BTT_refill_cube_sect(%brick, %up, %box);
		return;
	}
	%downVec = "0 0" SPC -%db.brickSizeZ * 0.2;
	%down = BTT_refill_findBrick(vectorAdd(%pos, %downVec), %box, %db.getName());
	if (isObject(%down)) {
		BTT_refill_cube_sect(%brick, %down, %box);
		return;
	}
}

// Combines a dirt brick with nearby dirt bricks.
// TODO: fix player getting stuck in brick (add velocity)
function BTT_refill(%brick) {
	%dbName = %brick.getDatablock().getName();
	%pos = %brick.position;
	if (%dbName $= "brick4x4DirtData" ||
	    %dbName $= "brick2x2DirtData" ||
	    %dbName $= "brick1x1DirtData") {
		BTT_refill_brick_small(%brick);
	}
	else if (%dbName $= "brick8x8DirtData") {
		BTT_refill_brick_8x8(%brick);
	} else if (%dbName $= "brick2xCubeDirtData"  ||
		 %dbName $= "brick4xCubeDirtData"  ||
		 %dbName $= "brick8xCubeDirtData"  ||
		 %dbName $= "brick16xCubeDirtData" ||
		 %dbName $= "brick32xCubeDirtData") {
		BTT_refill_cube(%brick);
	}
}
