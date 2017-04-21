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
	if(%dbName $= "brick2xCubeDirtData"  ||
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
