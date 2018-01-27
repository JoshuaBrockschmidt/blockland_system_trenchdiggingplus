////
// Objects to aid in the removing and adding of dirt chunks
////

// Dummy object that represents a brick
//
// @param datablock db		Datablock of dummy brick
// @param vector pos		Position of dummy brick
// @return TDP_DummyBrick	Newly created dummy brick object
function TDP_DummyBrick(%db, %pos) {
	%this = new ScriptGroup()
	{
		class = TDP_DummyBrick;
	        db = %db;
		position = %pos;
	};
	return %this;
}

// Checks if there are any bricks that would obstruct planting of dummy brick
//
// @return boolean	False if the brick would be obstructed, true if not.
function TDP_DummyBrick::hasRoom(%this) {
	%box = %this.db.brickSizeX * 0.5 SPC
		%this.db.brickSizeY * 0.5 SPC
		%this.db.brickSizeZ * 0.2;
	%box = vectorSub(%box, "0.1 0.1 0.1");
	initContainerBoxSearch(%this.position, %box, $TypeMasks::fxBrickObjectType);
	while(isObject(%brick = containerSearchNext()))
		if (%brick.isPlanted)
			return false;
	return true;
}

// Plants a brick in place of the dummy brick
//
// @param GameConnection client		Client to place in ownership of brick.
// @param int colorID			Color ID of brick.
// @return FxDTSBrick		        Newly planted brick.
function TDP_DummyBrick::plant(%this, %client, %colorID, %bg) {
	%newBrick = new fxDTSBrick()
        {
		client = %client;
		colorFxID = 0;
		colorID = %colorID;
		datablock = %this.db;
		position = %this.position;
		rotation = "1 0 0 0";
		shapeFxID = 0;
	};
	%newBrick.isPlanted = 1;
	%newBrick.setTrusted(1);
	%error = %newBrick.plant();
	if(%error && %error != 2) {
		%newBrick.delete();
	}
	else {
		%bg.add(%newBrick);

		// Make it so player objects do not fall through the new brick(s).
		%db = %newBrick.getDatablock();
		%displace = 0 SPC 0 SPC %db.brickSizeZ * 0.1 + 0.05;
		%pos = vectorAdd(%this.position, %displace);
		%box = %db.brickSizeX * 0.5 SPC %db.brickSizeY * 0.5 SPC 0.1;
		initContainerBoxSearch(%pos, %box, $TypeMasks::PlayerObjectType);
		while(isObject(%player = containerSearchNext())) {
			if (!%player.TDP_correctVel) {
				%player.addVelocity("0 0 2");
				%player.TDP_correctVel = 1;
				schedule(33, 0, TDP_uncorrectVel, %player);
			}
		}

		return %newBrick;
	}
}


// For use in planning out which bricks will remain and which will be kept
// after a cubic section of dirt is taken away.
//
// @param FxDTSBrick brick      Brick to be split.
function TDP_Chunk(%client, %brick) {
	%this = new ScriptGroup()
	{
		class = TDP_Chunk;
		brick = %brick;
		numTake = 0;
	};
	return %this;
}

// Splits dirt into a 2x2x1 chunk of dummy bricks, which are further
// fragmented as necessary. Helper function for TDP_Chunk::planFragments().
//
// @param datablock newDb       Datablock of new dummy bricks.
// @param vector midPos		Midpoint of area being split.
// @param vector boxPos		Position of box encompassing dirt to be taken.
// @param vector box		Dimensions of box encompassing dirt to be taken.
function TDP_Chunk::planFragments_split(%this, %newDb, %midPos, %boxPos, %box) {
	%shift = 0.25 * %newDb.brickSizeX;
	%displace1 = vectorScale("1 1 0", %shift);
	%displace2 = vectorScale("-1 1 0", %shift);
	%displace3 = vectorScale("1 -1 0", %shift);
	%displace4 = vectorScale("-1 -1 0", %shift);
	%newPos1 = vectorAdd(%midPos, %displace1);
	%newPos2 = vectorAdd(%midPos, %displace2);
	%newPos3 = vectorAdd(%midPos, %displace3);
	%newPos4 = vectorAdd(%midPos, %displace4);
	%this.planFragments(%newDb, %newPos1, %boxPos, %box);
	%this.planFragments(%newDb, %newPos2, %boxPos, %box);
	%this.planFragments(%newDb, %newPos3, %boxPos, %box);
	%this.planFragments(%newDb, %newPos4, %boxPos, %box);
}


// Figures out what bricks should be put in the place of the object's brick
// and stores them as dummy vectors for later reconstruction.
//
// @param datablock db	        Datablock of dummy brick being split.
// @param vector brickPos	Position of dummy brick being split.
// @param vector boxPos		Position of box encompassing dirt to be taken.
// @param vector box		Dimensions of box encompassing dirt to be taken.
function TDP_Chunk::planFragments(%this, %db, %brickPos, %boxPos, %box) {
	%db = %db.getName();
	switch$(%db) {
	case "brick8x16DirtData":
		%newDb = "brick8x8DirtData";

		// If a brick of this size is chosen, it must be colliding with the box.
		// Therefore, we will assume it needs to be split.
		%rotAngle = getWord(%this.brick.rotation, 3);
		%isNorth = (mFloor(%rotAngle / 90) + 1) % 2;
		%displace = %isNorth ? "0 2 0" : "2 0 0";
		%brickPos1 = vectorAdd(%brickPos, %displace);
		%brickPos2 = vectorSub(%brickPos, %displace);
		%this.planFragments(%newDb, %brickPos1, %boxPos, %box);
		%this.planFragments(%newDb, %brickPos2, %boxPos, %box);
	case "brick8x8DirtData":
		%newDb = "brick4x4DirtData";
		%checkBrick = true;
	case "brick4x4DirtData":
		%newDb = "brick2x2DirtData";
		%checkBrick = true;
	case "brick2x2DirtData":
		%newDb = "brick1x1DirtData";
		%checkBrick = true;
	case "brick1x1DirtData":
		%checkBrick = true;
	case "brick64xCubeDirtData":
		%newDb = "brick32xCubeDirtData";
		%checkCube = true;
	case "brick32xCubeDirtData":
		%newDb = "brick16xCubeDirtData";
		%checkCube = true;
        case "brick16xCubeDirtData":
		%newDb = "brick8xCubeDirtData";
		%checkCube = true;
        case "brick8xCubeDirtData":
		%newDb = "brick4xCubeDirtData";
		%checkCube = true;
        case "brick4xCubeDirtData":
		%newDb = "brick2xCubeDirtData";
		%checkCube = true;
        case "brick2xCubeDirtData":
		%checkCube = true;
	default:
		return;
	}

        if (%checkBrick) {
		%displace = vectorScale("0.25 0.25 0.3", %db.brickSizeX);
		%corner = vectorSub(%brickPos, %displace);
		%brickAABB = getWords(%corner, 0, 1) SPC
			 0.5 * %db.brickSizeX SPC
			 0.5 * %db.brickSizeX;
		%boxAABB = getWords(%boxPos, 0, 1) SPC getWords(%box, 0, 1);
		if (TDP_AABBAABB_2D(%brickAABB, %boxAABB)) {
			if (%db $= "brick1x1DirtData")
				%this.numTake++;
			else
				%this.planFragments_split(%newDb, %brickPos, %boxPos, %box);
		}
		else {
			%dummy = TDP_DummyBrick(%db, %brickPos);
			%this.add(%dummy);
		}
	}
	else if (%checkCube) {
		%displace = vectorScale("0.25 0.25 0.25", %db.brickSizeX);
		%corner = vectorSub(%brickPos, %displace);
		%brickAABB = %corner SPC vectorScale("0.5 0.5 0.5", %db.brickSizeX);
		%boxAABB = %boxPos SPC %box;
		if (TDP_AABBAABB_3D(%brickAABB, %boxAABB)) {
			if (%db $= "brick2xCubeDirtData") {
				// Remove brick
				%this.numTake++;
			}
			else {
				%shift = 0.25 * %newDb.brickSizeX;
				%displace = "0 0" SPC %shift;
				%brickPos1 = vectorAdd(%brickPos, %displace);
				%brickPos2 = vectorSub(%brickPos, %displace);
				%this.planFragments_split(%newDb, %brickPos1, %boxPos, %box);
				%this.planFragments_split(%newDb, %brickPos2, %boxPos, %box);
			}
		}
		else {
			%dummy = TDP_DummyBrick(%db, %brickPos);
			%this.add(%dummy);
		}
	}
}

// Deletes the old brick and recontructs it according to the dummy bricks
// created by TDP_Chunk::planFragments().
//
// @return string	Color IDs of taken dirt as a string of words.
function TDP_Chunk::rebuild(%this) {
	%client = %this.brick.client;
	%colorID = %this.brick.colorID;
	%bg = %this.brick.getGroup();
	%numDummy = %this.getCount();
	%colorID = %this.brick.getColorID();
	for (%i = 0; %i < %this.numTake; %i++)
		%colorIDs = %colorIDs SPC %colorID;
	%colorIDs = trim(%colorIDs);
	%this.brick.delete();
	for (%i = 0; %i < %numDummy; %i++) {
		%dummy = %this.getObject(%i);
		%dummy.plant(%client, %colorID, %bg);
	}
	%this.deleteAll();

	return %colorIDs;
}


// Removes dirt bricks and recreates broken dirt bricks.
//
// @return TDP_Chunker	Newly created chunker object.
function TDP_Chunker() {
	%this = new ScriptGroup() { class = TDP_Chunker; };
	return %this;
}

// Finds dirt bricks to take from.
//
// @param vector box	Dimensions of box encompassing dirt to be taken.
// @param vector boxPos	Position of box encompassing dirt to be taken.
function TDP_Chunker::findChunks(%this, %box, %boxPos) {
	%this.clear();
	%boxCorner = vectorSub(%boxPos, vectorScale(%box, 0.5));
	initContainerBoxSearch(%boxPos, %box, $TypeMasks::fxBrickObjectType);
	while (isObject(%brick = containerSearchNext())) {
		if (%brick.isPlanted && %brick.getDatablock().isTrenchDirt) {
			%newChunk = TDP_Chunk(%this.client, %brick);
			%db = %brick.getDatablock().getName();
			%brickPos = %brick.position;
			%newChunk.planFragments(%db, %brickPos, %boxCorner, %box);
			%this.add(%newChunk);
		}
	}
}

// Gets the total quantity of dirt that will be taken.
//
// @return int  Quantity of dirt that will be taken.
function TDP_Chunker::getTotalTake(%this) {
	%numChunks = %this.getCount();
	%totalTake = 0;
	for (%i = 0; %i < %numChunks; %i++)
		%totalTake += %this.getObject(%i).numTake;
	return %totalTake;
}

// Takes away dirt.
//
// @return string	Color IDs of taken dirt as a string of words.
function TDP_Chunker::take(%this) {
	%numChunks = %this.getCount();
	for (%i = 0; %i < %numChunks; %i++) {
		%chunk = %this.getObject(%i);
		%newColorIDs = %chunk.rebuild();
		%colorIDs = %colorIDs SPC %newColorIDs;
	}
	%colorIDs = trim(%colorIDs);
	%this.deleteAll();

	return %colorIDs;
}

// Places and merges dirt bricks.
//
// @param GameConnection client	Client who is placing bricks.
// @param vector pos	        Center position of cubic volume of dirt to be
//				placed.
// @param int cubeSize	        Side length (in number of dirt bricks) of cubic
//				volume of dirt to be placed.
// @param boolean isBrick	Whether or not dirt to be placed is bricks
//				(as opposed to cubes).
// @return TDP_Refiller		Newly created refiller object.
function TDP_Refiller(%pos, %cubeSize, %isBrick) {
	%this = new ScriptGroup()
        {
		class = TDP_Refiller;
		cubeSize = %cubeSize;
		isBrick = %isBrick;
		position = %pos;
	};
	return %this;
}

// Places dummy bricks in configuration specified by %this.cubeSize
// and %this.position
function TDP_Refiller::planPlacing(%this) {
	%this.deleteAll();
	if (%this.isBrick) {
		%incrX = 0.5;
		%incrY = 0.5;
		%incrZ = 0.6;
		%db = brick1x1DirtData;
	}
	else {
		%incrX = 1;
		%incrY = 1;
		%incrZ = 1;
		%db = brick2xCubeDirtData;
	}
	%displace = vectorScale(%incrX SPC %incrY SPC %incrZ, 0.5);
	%displace = vectorScale(%displace, %this.cubeSize - 1);
	%cornerPos = vectorSub(%this.position, %displace);
	%limitX = %this.cubeSize * %incrX;
	%limitY = %this.cubeSize * %incrY;
	%limitZ = %this.cubeSize * %incrZ;
	%numBricks = 0;
	for (%x = 0; %x < %limitX; %x += %incrX) {
		for (%y = 0; %y < %limitY; %y += %incrY) {
			for (%z = 0; %z < %limitZ; %z += %incrZ) {
				%brickPos = vectorAdd(%cornerPos, %x SPC %y SPC %z);
				%dummy = TDP_DummyBrick(%db, %brickPos);
				if (%dummy.hasRoom())
					%this.add(%dummy);
				else
					%dummy.delete();
			}
		}
	}
}

// Get quantity of dirt that will be placed.
//
// @return int	Quantity of dirt to be placed.
function TDP_Refiller::getNumPlace(%this) {
	return %this.getCount();
}

// Places bricks according to previously specified position and size.
//
// @param GameConnection brickCl	Client to place bricks in ownership of.
// @param SimSet bg			Brickgroup of newly placed bricks.
// @param GameConnection cl		Client placing dirt.
// @param string colorIDs		Color IDs of dirt bricks to be placed,
//					ordered left to right. If entries are
//					left empty, or variable itself is null,
//					the default dirt color will be used,
//					as defined by $TDP::defaultColor.
function TDP_Refiller::place(%this, %brickCl, %bg, %cl, %colorIDs) {
	%count = %this.getCount();
	for (%i = 0; %i < %count; %i++) {
		// TODO: add support for lack of %cl; use color of dirt being placed on
		%colorID = getWord(%colorIDs, %i);
		if (%colorID $= "")
			%colorID = $TDP::defaultColor;
		%cl.TDP_getDirtColorID(%i);
		%dummy = %this.getObject(%i);
		%newBricks[%i] = %dummy.plant(%cl, %colorID, %bg);
	}
	%this.deleteAll();
	for (%i = 0; %i < %count; %i++)
		if (isObject(%newBricks[%i]))
			TDP_refill(%newBricks[%i]);
}

// Does a container search for a brick of a particular datablock.
//
// @param vector pos	Position of container.
// @param vector box    Dimensions of container.
// @param datablock db  Datablock of brick to search for.
// @return int		Object ID of brick if one is found by itself,
//			otherwise -1 if no brick is found or if a brick is
//			found that is not of the specified datablock.
function TDP_refill_findBrick(%pos, %box, %db) {
	%db = %db.getName();
	initContainerBoxSearch(%pos, %box, $TypeMasks::fxBrickObjectType);
	%brick = containerSearchNext();
	while (isObject(%brick)) {
		if (!%brick.isPlanted)
			%brick = containerSearchNext();
		else if (isObject(%brick) &&
			 %brick.getDatablock().getName() $= %db &&
			 %brick.position $= %pos)
			return %brick;
		else 
			return -1;
	}
	return -1;
}

// Returns the most prevelant color among a vector of dirt bricks.
// In the case that no one color is most prevalent, the color of the
// first dirt brick in the array will be returned.
//
// @param string bricks	A word list of brick IDs.
// @return int		Colod ID of most prevalent color.
function TDP_refill_getColor(%bricks) {
	%numBricks = getWordCount(%bricks);
	%numColors = 0;
	// Tally the colors
	for (%b = 0; %b < %numBricks; %b++) {
		%thisColor = getWord(%bricks, %b).colorID;
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

// Replaces dirt bricks with a single brick, and attempt to merge them further
// if possible.
//
// @param string bricks Datablock of bricks, followed by bricks IDs, formatted
//			like "<datablock> <id1> <id2> <id3> ..."
// @param vector newPos	Position of new brick.
// @return boolean	True if new brick was successfully planted,
//			false otherwise.
function TDP_refill_replace(%bricks, %newPos) {
	%db = getWord(%bricks, 0).getDatablock().getName();
	%doRefill = 1;
	%rot = "1 0 0 0";
	if (%db $= "brick1x1DirtData") {
		%newDb = "brick2x2DirtData";
	}
	else if (%db $= "brick2x2DirtData") {
		%newDb = "brick4x4DirtData";
	}
	else if (%db $= "brick4x4DirtData") {
		%newDb = "brick8x8DirtData";
	}
	else if (%db $= "brick8x8DirtData") {
		%newDb = "brick8x16DirtData";
		%diff = vectorSub(getWord(%bricks, 0).position, getWord(%bricks, 1).position);
		if (getWord(%diff, 0) != 0)
			%rot = "0 0 1 90";
		%doRefill = 0;
	}
	else if (%db $= "brick2xCubeDirtData") {
		%newDb = "brick4xCubeDirtData";
	}
	else if (%db $= "brick4xCubeDirtData") {
		%newDb = "brick8xCubeDirtData";
	}
	else if (%db $= "brick8xCubeDirtData") {
		%newDb = "brick16xCubeDirtData";
	}
	else if (%db $= "brick16xCubeDirtData") {
		%newDb = "brick32xCubeDirtData";
	}
	else if (%db $= "brick32xCubeDirtData") {
		%newDb = "brick64xCubeDirtData";
		%doRefill = 0;
	}
	else {
		return;
	}
	%brick1 = getWord(%bricks, 0);
	%brickGroup = %brick1.getGroup();
	%colorID = TDP_refill_getColor(%bricks);
	%brickNum = getWordCount(%bricks);
	for (%b = 0; %b < %brickNum; %b++)
		getWord(%bricks, %b).delete();
	%newDirt = new fxDTSBrick() {
		client = %brick1.client;
		colorFxID = 0;
		colorID = %colorID;
		datablock = %newDb;
		position = %newPos;
		rotation = %rot;
		shapeFxID = 0;
	};
	%newDirt.isPlanted = 1;
	%newDirt.setTrusted(1);
	%error = %newDirt.plant();
	if(%error && %error != 2) {
		%newDirt.delete();
		%success = true;
	}
	else {
		%brickGroup.add(%newDirt);
		%success = false;
	}
	if (%doRefill)
		TDP_refill(%newDirt);
	return %success;
}

// Merges a 1x1, 2x2, or 4x4 dirt brick with nearby dirt bricks.
//
// @param FxDTSBrick brick	Brick to merge.
// @return boolean		True if brick was successfully merged,
//				false otherwise.
function TDP_refill_brick_small(%brick) {
	%db = %brick.getDatablock().getName();
	%size = %db.brickSizeX;
	%pos = %brick.position;
	%box = 0.5 * %size - 0.1
		 SPC 0.5 * %size - 0.1
		 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = TDP_refill_findBrick(vectorAdd(%pos, %northVec), %box, %db);
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = TDP_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %db);
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = TDP_refill_findBrick(vectorAdd(%pos, %westVec), %box, %db);
	if (isObject(%north)) {
		// northeast
		if (isObject(%east)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne = TDP_refill_findBrick(vectorAdd(%pos, %neVec), %box, %db);
			if (isObject(%ne)) {
				%bricks = %brick SPC %north SPC %ne SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 0.25 0", %size));
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
		// northwest
		if (isObject(%west)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw = TDP_refill_findBrick(vectorAdd(%pos, %nwVec), %box, %db);
			if (isObject(%nw)) {
				%bricks = %brick SPC %north SPC %nw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 0.25 0", %size));
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = TDP_refill_findBrick(vectorAdd(%pos, %southVec), %box, %db);
	if (isObject(%south)) {
		// southeast
		if (isObject(%east)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se = TDP_refill_findBrick(vectorAdd(%pos, %seVec), %box, %db);
			if (isObject(%se)) {
				%bricks = %brick SPC %south SPC %se SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 -0.25 0", %size));
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
		// southwest
		if (isObject(%west)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw = TDP_refill_findBrick(vectorAdd(%pos, %swVec), %box, %db);
			if (isObject(%sw)) {
				%bricks = %brick SPC %south SPC %sw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 -0.25 0", %size));
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
	}
	return false;
}

// Merges a 8x8 dirt brick with nearby dirt bricks.
//
// @param FxDTSBrick brick	Brick to merge.
// @return boolean		True if brick was successfully merged,
//				false otherwise.
function TDP_refill_brick_8x8(%brick) {
	%db = %brick.getDatablock().getName();
	%size = %db.brickSizeX;
	%pos = %brick.position;
	%box = 3.9 SPC 3.9 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = TDP_refill_findBrick(vectorAdd(%pos, %northVec), %box, %db);
	if (isObject(%north)) {
		%bricks = %brick SPC %north;
		%newPos = vectorAdd(%pos, "0 2 0");
		return TDP_refill_replace(%bricks, %newPos);
	}
	// east
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = TDP_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %db);
	if (isObject(%east)) {
		%bricks = %brick SPC %east;
		%newPos = vectorAdd(%pos, "2 0 0");
		return TDP_refill_replace(%bricks, %newPos);
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = TDP_refill_findBrick(vectorAdd(%pos, %southVec), %box, %db);
	if (isObject(%south)) {
		%bricks = %brick SPC %south;
		%newPos = vectorAdd(%pos, "0 -2 0");
		return TDP_refill_replace(%bricks, %newPos);
	}
	// west
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = TDP_refill_findBrick(vectorAdd(%pos, %westVec), %box, %db);
	if (isObject(%west)) {
		%bricks = %brick SPC %west;
		%newPos = vectorAdd(%pos, "-2 0 0");
		return TDP_refill_replace(%bricks, %newPos);
	}
	return false;
}

// Merges a cube dirt with cube dirt adjacent and either above or below it
// if possible.
//
// @param FxDTSBrick baseBrick	Brick that is being merged.
// @param FxDTSBrick brick2	Brick located either above or below base brick.
// @param vector box		Dimension of box to search for neighboring
//				brick with.
// @param boolean		True if the brick could be merged,
//				false otherwise.
function TDP_refill_cube_sect(%baseBrick, %brick2, %box) {
	%db = %baseBrick.getDatablock();
	%db = %db.getName();
	%pos1 = %baseBrick.position;
	%pos2 = %brick2.position;
	%northVec = vectorScale("0 0.5 0", %db.brickSizeX);
	%north1 = TDP_refill_findBrick(vectorAdd(%pos1, %northVec), %box, %db);
	%north2 = TDP_refill_findBrick(vectorAdd(%pos2, %northVec), %box, %db);
	%eastVec = vectorScale("0.5 0 0", %db.brickSizeX);
	%east1 = TDP_refill_findBrick(vectorAdd(%pos1, %eastVec), %box, %db);
	%east2 = TDP_refill_findBrick(vectorAdd(%pos2, %eastVec), %box, %db);
	%westVec = vectorScale("-0.5 0 0", %db.brickSizeX);
	%west1 = TDP_refill_findBrick(vectorAdd(%pos1, %westVec), %box, %db);
	%west2 = TDP_refill_findBrick(vectorAdd(%pos2, %westVec), %box, %db);
	// north
	if (isObject(%north1) && isObject(%north2)) {
		// northeast
		if (isObject(%east1) && isObject(%east2)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne1 = TDP_refill_findBrick(vectorAdd(%pos1, %neVec), %box, %db);
			%ne2 = TDP_refill_findBrick(vectorAdd(%pos2, %neVec), %box, %db);
			if (isObject(%ne1) && isObject(%ne2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					%north1 SPC %north2 SPC
					%ne1 SPC %ne2 SPC
				        %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%neVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
		// northwest
		if (isObject(%west1) && isObject(%west2)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw1 = TDP_refill_findBrick(vectorAdd(%pos1, %nwVec), %box, %db);
			%nw2 = TDP_refill_findBrick(vectorAdd(%pos2, %nwVec), %box, %db);
			if (isObject(%nw1) && isObject(%nw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
				        %north1 SPC %north2 SPC
					%nw1 SPC %nw2 SPC
					%west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%nwVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %db.brickSizeX);
	%south1 = TDP_refill_findBrick(vectorAdd(%pos1, %southVec), %box, %db);
	%south2 = TDP_refill_findBrick(vectorAdd(%pos2, %southVec), %box, %db);
	if (isObject(%south1) && isObject(%south2)) {
		// southeast
		if (isObject(%east1) && isObject(%east2)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se1 = TDP_refill_findBrick(vectorAdd(%pos1, %seVec), %box, %db);
			%se2 = TDP_refill_findBrick(vectorAdd(%pos2, %seVec), %box, %db);
			if (isObject(%se1) && isObject(%se2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %se1 SPC %se2 SPC
					 %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%seVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
		// southwest
		if (isObject(%west1) && isObject(%west2)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw1 = TDP_refill_findBrick(vectorAdd(%pos1, %swVec), %box, %db);
			%sw2 = TDP_refill_findBrick(vectorAdd(%pos2, %swVec), %box, %db);
			if (isObject(%sw1) && isObject(%sw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %sw1 SPC %sw2 SPC
					 %west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%swVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				return TDP_refill_replace(%bricks, %newPos);
			}
		}
	}
	return false;
}

// Merges a dirt cube with nearby dirt cubes.
//
// @param FxDTSBrick brick	Brick to merge.
// @return boolean		True if brick was successfully merged,
//				false otherwise.
function TDP_refill_cube(%brick) {
	%db = %brick.getDatablock();
	%pos = %brick.position;
	%box = %db.brickSizeX * 0.5 SPC %db.brickSizeY * 0.5 SPC %db.brickSizeZ * 0.2;
	%box = vectorSub(%box, "0.1 0.1 0.1");
	%upVec = "0 0" SPC %db.brickSizeZ * 0.2;
	%up = TDP_refill_findBrick(vectorAdd(%pos, %upVec), %box, %db.getName());
	if (isObject(%up))
		if (TDP_refill_cube_sect(%brick, %up, %box))
			return true;
	%downVec = "0 0" SPC -%db.brickSizeZ * 0.2;
	%down = TDP_refill_findBrick(vectorAdd(%pos, %downVec), %box, %db.getName());
	if (isObject(%down))
		return TDP_refill_cube_sect(%brick, %down, %box);
	return false;
}

// Merges a dirt brick with nearby dirt bricks.
//
// @param FxDTSBrick brick	Brick to merge.
function TDP_refill(%brick) {
	%db = %brick.getDatablock().getName();
	%pos = %brick.position;
	if (%db $= "brick4x4DirtData" ||
	    %db $= "brick2x2DirtData" ||
	    %db $= "brick1x1DirtData") {
		TDP_refill_brick_small(%brick);
	}
	else if (%db $= "brick8x8DirtData") {
		TDP_refill_brick_8x8(%brick);
	} else if (%db $= "brick2xCubeDirtData"  ||
		 %db $= "brick4xCubeDirtData"  ||
		 %db $= "brick8xCubeDirtData"  ||
		 %db $= "brick16xCubeDirtData" ||
		 %db $= "brick32xCubeDirtData") {
		TDP_refill_cube(%brick);
	}
}
