////
// Assorted functions that do not belong in any one file.
////

function vectorFloor(%vec) {
	%newVec = mFloor(getWord(%vec, 0))
		SPC mFloor(getWord(%vec, 1))
		SPC mFloor(getWord(%vec, 2));
	return %newVec;
}

function TRT_isDirtCube(%brick) {
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

function TRT_isDirtBrick(%brick) {
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

// Checks if two 2D AABB are colliding or touching.
// AABB is a vector consisting of (x, y, width, height).
// Returns 1 if the AABBs collide, and 0 if they do not.
function TRT_AABBAABB_2D(%AABB1, %AABB2) {
	%x1 = getWord(%AABB1, 0);
	%y1 = getWord(%AABB1, 1);
	%w1 = getWord(%AABB1, 2);
	%h1 = getWord(%AABB1, 3);
	%x2 = getWord(%AABB2, 0);
	%y2 = getWord(%AABB2, 1);
	%w2 = getWord(%AABB2, 2);
	%h2 = getWord(%AABB2, 3);
	if (%x1 > %x2 + %w2 ||
	    %x1 + %w1 < %x2 ||
	    %y1 > %y2 + %h2 ||
	    %y1 + %h1 < %y2)
		return 0;
	else
		return 1;
}

// Checks if two cube-shaped AABB are colliding or touching.
// AABB is a vector consisting of (x, y, z, size x, size y, size z).
// Returns 1 if the AABBs collide, and 0 if they do not.
function TRT_AABBAABB_3D(%AABB1, %AABB2) {
	%x1 = getWord(%AABB1, 0);
	%y1 = getWord(%AABB1, 1);
	%z1 = getWord(%AABB1, 2);
	%sizeX1 = getWord(%AABB1, 3);
	%sizeY1 = getWord(%AABB1, 4);
	%sizeZ1 = getWord(%AABB1, 5);
	%x2 = getWord(%AABB2, 0);
	%y2 = getWord(%AABB2, 1);
	%z2 = getWord(%AABB2, 2);
	%sizeX2 = getWord(%AABB2, 3);
	%sizeY2 = getWord(%AABB2, 4);
	%sizeZ2 = getWord(%AABB2, 5);
	if (%x1 > %x2 + %sizeX2 ||
	    %x1 + %sizeX1 < %x2 ||
	    %y1 > %y2 + %sizeY2 ||
	    %y1 + %sizeY1 < %y2 ||
	    %z1 > %z2 + %sizeZ2 ||
	    %z1 + %sizeZ1 < %z2)
		return 0;
	else
		return 1;
}

// Checks if a point lies inside a 3D AABB.
// The point is just a vector.
// AABB is a vector consisting of (x, y, z, size x, size y, size z).
// Returns 1 if the AABBs collide, and 0 if they do not.
function TRT_PointAABB_3D(%point, %AABB) {
	%xp = getWord(%point, 0);
	%yp = getWord(%point, 1);
	%zp = getWord(%point, 2);
	%xb = getWord(%AABB, 0);
	%yb = getWord(%AABB, 1);
	%zb = getWord(%AABB, 2);
	%sizeXb = getWord(%AABB, 3);
	%sizeYb = getWord(%AABB, 4);
	%sizeZb = getWord(%AABB, 5);
	if (%xp > %xb + %sizeXb ||
	    %xp < %xb           ||
	    %yp > %yb + %sizeYb ||
	    %yp < %yb           ||
	    %zp > %zb + %sizeZb ||
	    %zp < %zb)
		return 0;
	else
		return 1;
}

function TRT_uncorrectVel(%player) {
	%player.TRT_correctVel = 0;
}

function TRT_dummyBrick(%db, %pos) {
	%this = new ScriptGroup()
	{
		class = TRT_dummyBrick;
	        db = %db;
		position = %pos;
	};
	return %this;
}

function TRT_dummyBrick::hasRoom(%this) {
	%box = %this.db.brickSizeX * 0.5 SPC
		%this.db.brickSizeY * 0.5 SPC
		%this.db.brickSizeZ * 0.2;
	%box = vectorSub(%box, "0.1 0.1 0.1");
	initContainerBoxSearch(%this.position, %box, $TypeMasks::fxBrickObjectType);
	while(isObject(%brick = containerSearchNext()))
		if (%brick.isPlanted)
			return 0;
	return 1;
}

function TRT_dummyBrick::plant(%this, %client, %colorId, %bg) {
	%newBrick = new fxDTSBrick()
        {
		client = %client;
		colorFxId = 0;
		colorId = %colorId;
		datablock = %this.db;
		position = %this.position;
		rotation = "1 0 0 0";
		shapeFxId = 0;
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
			if (!%player.TRT_correctVel) {
				%player.addVelocity("0 0 2");
				%player.TRT_correctVel = 1;
				schedule(33, 0, TRT_uncorrectVel, %player);
			}
		}

		return %newBrick;
	}
}

// Object for taking a chunk from a dirt brick.
function TRT_chunk(%client, %brick) {
	%this = new ScriptGroup()
	{
		class = TRT_chunk;
		client = %client;
		brick = %brick;
		numTake = 0;
	};
	return %this;
}

function TRT_chunk::planFragments_split(%this, %newDbName, %brickPos, %boxPos, %boxDim, %shift) {
	%displace1 = vectorScale("1 1 0", %shift);
	%displace2 = vectorScale("-1 1 0", %shift);
	%displace3 = vectorScale("1 -1 0", %shift);
	%displace4 = vectorScale("-1 -1 0", %shift);
	%newPos1 = vectorAdd(%brickPos, %displace1);
	%newPos2 = vectorAdd(%brickPos, %displace2);
	%newPos3 = vectorAdd(%brickPos, %displace3);
	%newPos4 = vectorAdd(%brickPos, %displace4);
	%this.planFragments(%newDbName, %newPos1, %boxPos, %boxDim);
	%this.planFragments(%newDbName, %newPos2, %boxPos, %boxDim);
	%this.planFragments(%newDbName, %newPos3, %boxPos, %boxDim);
	%this.planFragments(%newDbName, %newPos4, %boxPos, %boxDim);
}

// Figures out what bricks should be put in the place of the object's brick
// and stores them as dummy vectors for later reconstruction.
function TRT_chunk::planFragments(%this, %dbName, %brickPos, %boxPos, %boxDim) {
	if (%dbName $= "brick8x16DirtData") {
		// If a brick of this size is chosen, it must be colliding with the box.
		// Therefore, we will assume it needs to be split.
		%rotAngle = getWord(%this.brick.rotation, 3);
		%isNorth = (mFloor(%rotAngle / 90) + 1) % 2;
		%displace = %isNorth ? "0 2 0" : "2 0 0";
		%brickPos1 = vectorAdd(%brickPos, %displace);
		%brickPos2 = vectorSub(%brickPos, %displace);
		%this.planFragments("brick8x8DirtData", %brickPos1, %boxPos, %boxDim);
		%this.planFragments("brick8x8DirtData", %brickPos2, %boxPos, %boxDim);
	}
	else if (%dbName $= "brick8x8DirtData" ||
		 %dbName $= "brick4x4DirtData" ||
		 %dbName $= "brick2x2DirtData" ||
		 %dbName $= "brick1x1DirtData") {
		if (%dbName $= "brick8x8DirtData")
			%newDbName = "brick4x4DirtData";
		else if (%dbName $= "brick4x4DirtData")
			%newDbName = "brick2x2DirtData";
		else
			%newDbName = "brick1x1DirtData";

		%displace = vectorScale("0.25 0.25 0.3", %dbName.brickSizeX);
		%corner = vectorSub(%brickPos, %displace);
		%brickAABB = getWords(%corner, 0, 1) SPC
			 0.5 * %dbName.brickSizeX SPC
			 0.5 * %dbName.brickSizeX;
		%boxAABB = getWords(%boxPos, 0, 1) SPC getWords(%boxDim, 0, 1);
		if (TRT_AABBAABB_2D(%brickAABB, %boxAABB)) {
			if (%dbName $= "brick1x1DirtData") {
				%this.numTake++;
			}
			else {
				%shift = 0.25 * %newDbName.brickSizeX;
				%this.planFragments_split(%newDbName, %brickPos,
							  %boxPos, %boxDim, %shift);
			}
		}
		else {
			%dummy = TRT_dummyBrick(%dbName, %brickPos);
			%this.add(%dummy);
		}
	}
	else if (%dbName $= "brick64xCubeDirtData" ||
		 %dbName $= "brick32xCubeDirtData" ||
		 %dbName $= "brick16xCubeDirtData" ||
		 %dbName $= "brick8xCubeDirtData"  ||
		 %dbName $= "brick4xCubeDirtData"  ||
		 %dbName $= "brick2xCubeDirtData") {
		if (%dbName $= "brick64xCubeDirtData")
			%newDbName = "brick32xCubeDirtData";
		else if (%dbNAme $= "brick32xCubeDirtData")
			%newDbName = "brick16xCubeDirtData";
		else if (%dbName $= "brick16xCubeDirtData")
			%newDbName = "brick8xCubeDirtData";
		else if (%dbName $= "brick8xCubeDirtData")
			%newDbName = "brick4xCubeDirtData";
		else
			%newDbName = "brick2xCubeDirtData";
		%displace = vectorScale("0.25 0.25 0.25", %dbName.brickSizeX);
		%corner = vectorSub(%brickPos, %displace);
		%brickAABB = %corner SPC vectorScale("0.5 0.5 0.5", %dbName.brickSizeX);
		%boxAABB = %boxPos SPC %boxDim;
		if (TRT_AABBAABB_3D(%brickAABB, %boxAABB)) {
			if (%dbName $= "brick2xCubeDirtData") {
				%this.numTake++;
			}
			else {
				%shift = 0.25 * %newDbName.brickSizeX;
				%displace = "0 0" SPC %shift;
				%brickPos1 = vectorAdd(%brickPos, %displace);
				%brickPos2 = vectorSub(%brickPos, %displace);
				%this.planFragments_split(%newDbName, %brickPos1,
							  %boxPos, %boxDim, %shift);
				%this.planFragments_split(%newDbName, %brickPos2,
							  %boxPos, %boxDim, %shift);
			}
		}
		else {
			%dummy = TRT_dummyBrick(%dbName, %brickPos);
			%this.add(%dummy);
		}
	}
}

// Deletes the old brick and recontructs it according to the dummy vectors
// created by TRT_chunk::chunk().
function TRT_chunk::rebuild(%this, %boxPos, %boxDim) {
	%client = %this.brick.client;
	%colorId = %this.brick.colorId;
	%bg = %this.brick.getGroup();
	%numDummy = %this.getCount();
	%colorID = %this.brick.getColorID();
	for (%i = 0; %i < %this.numTake; %i++)
		%colorIDs = %colorIDs SPC %colorID;
	%colorIDs = trim(%colorIDs);
	%this.brick.delete();
	for (%i = 0; %i < %numDummy; %i++) {
		%dummy = %this.getObject(%i);
		%dummy.plant(%client, %colorId, %bg);
	}
	%this.deleteAll();

	return %colorIDs;
}

// Holds multiple chunks.
function TRT_chunker(%client, %brick) {
	%this = new ScriptGroup()
        {
		class = TRT_chunker;
		client = %client;
	};
	return %this;
}

function TRT_chunker::findChunks(%this, %box, %boxPos) {
	%this.clear();
	%boxCorner = vectorSub(%boxPos, vectorScale(%box, 0.5));
	initContainerBoxSearch(%boxPos, %box, $TypeMasks::fxBrickObjectType);
	while (isObject(%brick = containerSearchNext())) {
		if (%brick.isPlanted && %brick.getDatablock().isTrenchDirt) {
			%newChunk = TRT_chunk(%this.client, %brick);
			%dbName = %brick.getDatablock().getName();
			%brickPos = %brick.position;
			%newChunk.planFragments(%dbName, %brickPos, %boxCorner, %box);
			%this.add(%newChunk);
		}
	}
}

function TRT_chunker::getTotalTake(%this) {
	%numChunks = %this.getCount();
	%totalTake = 0;
	for (%i = 0; %i < %numChunks; %i++)
		%totalTake += %this.getObject(%i).numTake;
	return %totalTake;
}

function TRT_chunker::take(%this) {
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

function TRT_refiller(%client, %pos, %isBrick) {
	%this = new ScriptGroup()
        {
		class = TRT_refiller;
		client = %client;
		position = %pos;
		isBrick = %isBrick;
	};
	return %this;
}

function TRT_refiller::planPlacing(%this) {
	%this.deleteAll();
	%cubeSize = %this.client.TRT_cubeSize;
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
	%displace = vectorScale(%displace, %cubeSize - 1);
	%cornerPos = vectorSub(%this.position, %displace);
	%limitX = %cubeSize * %incrX;
	%limitY = %cubeSize * %incrY;
	%limitZ = %cubeSize * %incrZ;
	%numBricks = 0;
	for (%x = 0; %x < %limitX; %x += %incrX) {
		for (%y = 0; %y < %limitY; %y += %incrY) {
			for (%z = 0; %z < %limitZ; %z += %incrZ) {
				%brickPos = vectorAdd(%cornerPos, %x SPC %y SPC %z);
				%dummy = TRT_dummyBrick(%db, %brickPos);
				if (%dummy.hasRoom())
					%this.add(%dummy);
				else
					%dummy.delete();
			}
		}
	}
}

function TRT_refiller::getNumPlace(%this) {
	return %this.getCount();
}

function TRT_refiller::place(%this, %brickClient, %bg) {
	%count = %this.getCount();
	for (%i = 0; %i < %count; %i++) {
		%colorId = %brickClient.TRT_getDirtColor(%i);
		%dummy = %this.getObject(%i);
		%newBricks[%i] = %dummy.plant(%brickClient, %colorId, %bg);
	}
	%this.deleteAll();
	for (%i = 0; %i < %count; %i++)
		if (isObject(%newBricks[%i]))
			TRT_refill(%newBricks[%i]);
}

function TRT_refill_findBrick(%pos, %box, %dbName) {
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
function TRT_refill_getColor(%bricks) {
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

function TRT_refill_replace(%bricks, %newPos) {
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
	%colorId = TRT_refill_getColor(%bricks);
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
		TRT_refill(%newDirt);
}

function TRT_refill_brick_small(%brick) {
	%dbName = %brick.getDatablock().getName();
	%size = %dbName.brickSizeX;
	%pos = %brick.position;
	%box = 0.5 * %size - 0.1
		 SPC 0.5 * %size - 0.1
		 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = TRT_refill_findBrick(vectorAdd(%pos, %northVec), %box, %dbName);
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = TRT_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %dbName);
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = TRT_refill_findBrick(vectorAdd(%pos, %westVec), %box, %dbName);
	if (isObject(%north)) {
		// northeast
		if (isObject(%east)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne = TRT_refill_findBrick(vectorAdd(%pos, %neVec), %box, %dbName);
			if (isObject(%ne)) {
				%bricks = %brick SPC %north SPC %ne SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 0.25 0", %size));
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// northwest
		if (isObject(%west)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw = TRT_refill_findBrick(vectorAdd(%pos, %nwVec), %box, %dbName);
			if (isObject(%nw)) {
				%bricks = %brick SPC %north SPC %nw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 0.25 0", %size));
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = TRT_refill_findBrick(vectorAdd(%pos, %southVec), %box, %dbName);
	if (isObject(%south)) {
		// southeast
		if (isObject(%east)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se = TRT_refill_findBrick(vectorAdd(%pos, %seVec), %box, %dbName);
			if (isObject(%se)) {
				%bricks = %brick SPC %south SPC %se SPC %east;
				%newPos = vectorAdd(%pos, vectorScale("0.25 -0.25 0", %size));
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// southwest
		if (isObject(%west)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw = TRT_refill_findBrick(vectorAdd(%pos, %swVec), %box, %dbName);
			if (isObject(%sw)) {
				%bricks = %brick SPC %south SPC %sw SPC %west;
				%newPos = vectorAdd(%pos, vectorScale("-0.25 -0.25 0", %size));
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
}

function TRT_refill_brick_8x8(%brick) {
	%dbName = %brick.getDatablock().getName();
	%size = %dbName.brickSizeX;
	%pos = %brick.position;
	%box = 3.9 SPC 3.9 SPC 0.5;
	// north
	%northVec = vectorScale("0 0.5 0", %size);
	%north = TRT_refill_findBrick(vectorAdd(%pos, %northVec), %box, %dbName);
	if (isObject(%north)) {
		%bricks = %brick SPC %north;
		%newPos = vectorAdd(%pos, "0 2 0");
		TRT_refill_replace(%bricks, %newPos);
		return;
	}
	// east
	%eastVec = vectorScale("0.5 0 0", %size);
	%east = TRT_refill_findBrick(vectorAdd(%pos, %eastVec), %box, %dbName);
	if (isObject(%east)) {
		%bricks = %brick SPC %east;
		%newPos = vectorAdd(%pos, "2 0 0");
		TRT_refill_replace(%bricks, %newPos);
		return;
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %size);
	%south = TRT_refill_findBrick(vectorAdd(%pos, %southVec), %box, %dbName);
	if (isObject(%south)) {
		%bricks = %brick SPC %south;
		%newPos = vectorAdd(%pos, "0 -2 0");
		TRT_refill_replace(%bricks, %newPos);
		return;
	}
	// west
	%westVec = vectorScale("-0.5 0 0", %size);
	%west = TRT_refill_findBrick(vectorAdd(%pos, %westVec), %box, %dbName);
	if (isObject(%west)) {
		%bricks = %brick SPC %west;
		%newPos = vectorAdd(%pos, "-2 0 0");
		TRT_refill_replace(%bricks, %newPos);
		return;
	}
}

function TRT_refill_cube_sect(%baseBrick, %brick2, %box) {
	%db = %baseBrick.getDatablock();
	%dbName = %db.getName();
	%pos1 = %baseBrick.position;
	%pos2 = %brick2.position;
	%northVec = vectorScale("0 0.5 0", %db.brickSizeX);
	%north1 = TRT_refill_findBrick(vectorAdd(%pos1, %northVec), %box, %dbName);
	%north2 = TRT_refill_findBrick(vectorAdd(%pos2, %northVec), %box, %dbName);
	%eastVec = vectorScale("0.5 0 0", %db.brickSizeX);
	%east1 = TRT_refill_findBrick(vectorAdd(%pos1, %eastVec), %box, %dbName);
	%east2 = TRT_refill_findBrick(vectorAdd(%pos2, %eastVec), %box, %dbName);
	%westVec = vectorScale("-0.5 0 0", %db.brickSizeX);
	%west1 = TRT_refill_findBrick(vectorAdd(%pos1, %westVec), %box, %dbName);
	%west2 = TRT_refill_findBrick(vectorAdd(%pos2, %westVec), %box, %dbName);
	// north
	if (isObject(%north1) && isObject(%north2)) {
		// northeast
		if (isObject(%east1) && isObject(%east2)) {
			%neVec = vectorAdd(%northVec, %eastVec);
			%ne1 = TRT_refill_findBrick(vectorAdd(%pos1, %neVec), %box, %dbName);
			%ne2 = TRT_refill_findBrick(vectorAdd(%pos2, %neVec), %box, %dbName);
			if (isObject(%ne1) && isObject(%ne2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					%north1 SPC %north2 SPC
					%ne1 SPC %ne2 SPC
				        %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%neVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// northwest
		if (isObject(%west1) && isObject(%west2)) {
			%nwVec = vectorAdd(%northVec, %westVec);
			%nw1 = TRT_refill_findBrick(vectorAdd(%pos1, %nwVec), %box, %dbName);
			%nw2 = TRT_refill_findBrick(vectorAdd(%pos2, %nwVec), %box, %dbName);
			if (isObject(%nw1) && isObject(%nw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
				        %north1 SPC %north2 SPC
					%nw1 SPC %nw2 SPC
					%west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%nwVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
	// south
	%southVec = vectorScale("0 -0.5 0", %db.brickSizeX);
	%south1 = TRT_refill_findBrick(vectorAdd(%pos1, %southVec), %box, %dbName);
	%south2 = TRT_refill_findBrick(vectorAdd(%pos2, %southVec), %box, %dbName);
	if (isObject(%south1) && isObject(%south2)) {
		// southeast
		if (isObject(%east1) && isObject(%east2)) {
			%seVec = vectorAdd(%southVec, %eastVec);
			%se1 = TRT_refill_findBrick(vectorAdd(%pos1, %seVec), %box, %dbName);
			%se2 = TRT_refill_findBrick(vectorAdd(%pos2, %seVec), %box, %dbName);
			if (isObject(%se1) && isObject(%se2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %se1 SPC %se2 SPC
					 %east1 SPC %east2;
				%displace = vectorScale(vectorAdd(%seVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
		// southwest
		if (isObject(%west1) && isObject(%west2)) {
			%swVec = vectorAdd(%southVec, %westVec);
			%sw1 = TRT_refill_findBrick(vectorAdd(%pos1, %swVec), %box, %dbName);
			%sw2 = TRT_refill_findBrick(vectorAdd(%pos2, %swVec), %box, %dbName);
			if (isObject(%sw1) && isObject(%sw2)) {
				%bricks = %baseBrick SPC %brick2 SPC
					 %south1 SPC %south2 SPC
					 %sw1 SPC %sw2 SPC
					 %west1 SPC %west2;
				%displace = vectorScale(vectorAdd(%swVec, vectorSub(%pos2, %pos1)), 0.5);
				%newPos = vectorAdd(%pos1, %displace);
				TRT_refill_replace(%bricks, %newPos);
				return;
			}
		}
	}
}

function TRT_refill_cube(%brick) {
	%db = %brick.getDatablock();
	%pos = %brick.position;
	%box = %db.brickSizeX * 0.5 SPC %db.brickSizeY * 0.5 SPC %db.brickSizeZ * 0.2;
	%box = vectorSub(%box, "0.1 0.1 0.1");
	%upVec = "0 0" SPC %db.brickSizeZ * 0.2;
	%up = TRT_refill_findBrick(vectorAdd(%pos, %upVec), %box, %db.getName());
	if (isObject(%up)) {
		TRT_refill_cube_sect(%brick, %up, %box);
		return;
	}
	%downVec = "0 0" SPC -%db.brickSizeZ * 0.2;
	%down = TRT_refill_findBrick(vectorAdd(%pos, %downVec), %box, %db.getName());
	if (isObject(%down)) {
		TRT_refill_cube_sect(%brick, %down, %box);
		return;
	}
}

// Combines a dirt brick with nearby dirt bricks.
function TRT_refill(%brick) {
	%dbName = %brick.getDatablock().getName();
	%pos = %brick.position;
	if (%dbName $= "brick4x4DirtData" ||
	    %dbName $= "brick2x2DirtData" ||
	    %dbName $= "brick1x1DirtData") {
		TRT_refill_brick_small(%brick);
	}
	else if (%dbName $= "brick8x8DirtData") {
		TRT_refill_brick_8x8(%brick);
	} else if (%dbName $= "brick2xCubeDirtData"  ||
		 %dbName $= "brick4xCubeDirtData"  ||
		 %dbName $= "brick8xCubeDirtData"  ||
		 %dbName $= "brick16xCubeDirtData" ||
		 %dbName $= "brick32xCubeDirtData") {
		TRT_refill_cube(%brick);
	}
}
