////
// General functions that don't belong in any other file
////

// Floors the values of a vector.
//
// @param vector vec	Vector to floor.
function vectorFloor(%vec) {
	%newVec = mFloor(getWord(%vec, 0))
		SPC mFloor(getWord(%vec, 1))
		SPC mFloor(getWord(%vec, 2));
	return %newVec;
}

// Checks if a string is a valid number.
// Copied from https://www.garagegames.com/community/forums/viewthread/133985
// Thanks to Michael Hall for posting the code. A slight correction was made.
//
// @param string str    String to validate.
// @return		True if string represents a valid number,
//			false otherwise.
function strIsNum(%str) {
	if (%str $= "")
		return false;
	%dots = 0;
	for(%i = 0; (%char = getSubStr(%str, %i, 1)) !$= ""; %i++) {
		switch$(%char) {
		case "0" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9":
			continue;
		case ".":
			if (%dots >= 1)
				return false;
			%dots++;
			continue;
		case "-":
			if (%i) // Only valid as first character.
				return false;
			continue;
		case "+":
			if (%i) // Only valid as first character.
				return false;
			continue;
		default:
			return false;
		}
	}
	return true;
}

// Checks if a brick is a dirt cube.
//
// @param FxDTSBrick brick	Brick to check.
// @return boolean		True if brick is a dirt cube, false otherwise.
function TDP_isDirtCube(%brick) {
	%dbName = %brick.getDatablock().getName();
	if (%dbName $= "brick2xCubeDirtData"  ||
	    %dbName $= "brick4xCubeDirtData"  ||
	    %dbName $= "brick8xCubeDirtData"  ||
	    %dbName $= "brick16xCubeDirtData" ||
	    %dbName $= "brick32xCubeDirtData" ||
	    %dbName $= "brick64xCubeDirtData") {
		return true;
	} else {
		return false;
	}
}

// Checks if a brick is a dirt brick.
//
// @param FxDTSBrick brick	Brick to check.
// @return boolean		True if brick is a dirt brick, false otherwise.
function TDP_isDirtBrick(%brick) {
	%dbName = %brick.getDatablock().getName();
	if (%dbName $= "brick8x16DirtData" ||
	    %dbName $= "brick8x8DirtData"  ||
	    %dbName $= "brick4x4DirtData"  ||
	    %dbName $= "brick2x2DirtData"  ||
	    %dbName $= "brick1x1DirtData") {
		return true;
	} else {
		return false;
	}
}

// Checks if two 2D AABB are colliding or touching.
//
// @param string AABB1	First AABB, containing the words
//			"<x> <y> <width> <height>".
// @param string AABB2	Second AABB, defined with the same format as %AABB1.
// @return boolean	True if the AABBs collide, false otherwise.
function TDP_AABBAABB_2D(%AABB1, %AABB2) {
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
		return false;
	else
		return true;
}

// Checks if two 3D AABB are colliding or touching.
//
// @param string AABB1	First AABB, containing the words
//			"<x> <y> <z> <size x> <size y> <size z>".
// @param string AABB2	Second AABB, defined with the same format as %AABB1.
// @return boolean	True if the AABBs collide, false otherwise.
function TDP_AABBAABB_3D(%AABB1, %AABB2) {
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
//
// @param vector point	Point to check.
// @param string AABB   AABB, containing the words
//			"<x> <y> <z> <size x> <size y> <size z>".
// @return boolean      True if the point lies inside the AABB, false otherwise.
function TDP_pointAABB_3D(%point, %AABB) {
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
		return false;
	else
		return true;
}
