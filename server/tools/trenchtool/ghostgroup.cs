////
// Provides the ghost brick object that shows where a player is going to
// dig or place.
////

function TRT_ghostGroup(%client, %size, %pos, %isBrick) {
	%this = new ScriptGroup()
		{
			class = TRT_ghostGroup;
			client = %client;
			colorId = %client.TRT_getDirtColorID();
			numBricks = 0;
			position = %pos;
			isBrick = %isBrick;
		};
	%this.setSize(%size);
        ServerGroup.add(%this);
	return %this;
}

function TRT_ghostGroup::updateColor(%this) {
	%clColorId = %this.client.TRT_getDirtColor();
	if (%this.colorId != %clColorId) {
		%this.colorId = %clColorId;
		for (%i = 0; %i < %this.numBricks; %i++)
			%this.bricks[%i].setColor(%this.colorId);
	}
}

function TRT_ghostGroup::getBasePos_bricks(%this) {
	%basePos = vectorAdd(vectorSub(%this.position,
				       vectorScale("0.5 0.5 0.6", %this.size / 2)),
			     "0.25 0.25 0.3");
	return %basePos;
}

function TRT_ghostGroup::getBasePos_cubes(%this) {
	%basePos = vectorAdd(vectorSub(%this.position,
				       vectorScale("1 1 1", %this.size / 2)),
			     "0.5 0.5 0.5");
	return %basePos;
}

function TRT_ghostGroup::setSize(%this, %size) {
	if (%this.size == %size)
		return;
	%this.size = %size;
	if (%this.numBricks != 0)
		%this.deleteBricks();
	if (%this.isBrick) {
		%basePos = %this.getBasePos_bricks();
		%baseDisplace = vectorSub(%this.position, %basePos);
		%limit8x = mFloor(%size / 8) * 4;
		%limit4x = mFloor(%size / 4) * 2;
		%limit2x = mFloor(%size / 2);
		%limit1x = %size / 2;
		for (%z = 0; %z < %size * 0.6; %z += 0.6) {
			for (%x = 0; %x < %limit8x; %x += 4) {
				for (%y = 0; %y < %limit8x; %y += 4) {
					%displace = %x+1.75 SPC %y+1.75 SPC %z;
					%newPos = vectorAdd(%basePos, %displace);
					%this.bricks[%this.numBricks] = new fxDTSBrick() {
						datablock = "brick8x8DirtData";
						colorID = %this.colorId;
						colorFxID = 0;
						shapeFxID = 0;
						isPlanted = 0;
						position = %newPos;
						displace = vectorSub(%displace, %baseDisplace);
					};
					%this.numBricks += 1;
				}
			}
			for (%x = 0; %x < %limit4x; %x += 2) {
				for (%y = 0; %y < %limit4x; %y += 2) {
					if (%x >= %limit8x || %y >= %limit8x) {
						%displace = %x+0.75 SPC %y+0.75 SPC %z;
						%newPos = vectorAdd(%basePos, %displace);
						%this.bricks[%this.numBricks] = new fxDTSBrick() {
							datablock = "brick4x4DirtData";
							colorID = %this.colorId;
							colorFxID = 0;
							shapeFxID = 0;
							isPlanted = 0;
							position = %newPos;
							displace = vectorSub(%displace, %baseDisplace);
						};
						%this.numBricks += 1;
					}
				}
			}
			for (%x = 0; %x < %limit2x; %x++) {
				for (%y = 0; %y < %limit2x; %y++) {
					if (%x >= %limit4x || %y >= %limit4x) {
						%displace = %x+0.25 SPC %y+0.25 SPC %z;
						%newPos = vectorAdd(%basePos, %displace);
						%this.bricks[%this.numBricks] = new fxDTSBrick() {
							datablock = "brick2x2DirtData";
							colorID = %this.colorId;
							colorFxID = 0;
							shapeFxID = 0;
							isPlanted = 0;
							position = %newPos;
							displace = vectorSub(%displace, %baseDisplace);
						};
						%this.numBricks += 1;
					}
				}
			}
			for (%x = 0; %x < %limit1x; %x += 0.5) {
				for (%y = 0; %y < %limit1x; %y += 0.5) {
					if (%x >= %limit2x || %y >= %limit2x) {
						%displace = %x SPC %y SPC %z;
						%newPos = vectorAdd(%basePos, %displace);
						%this.bricks[%this.numBricks] = new fxDTSBrick() {
							datablock = "brick1x1DirtData";
							colorID = %this.colorId;
							colorFxID = 0;
							shapeFxID = 0;
							isPlanted = 0;
							position = %newPos;
							displace = vectorSub(%displace, %baseDisplace);
						};
						%this.numBricks += 1;
					}
				}
			}
		}
	} else {
		%basePos = %this.getBasePos_cubes();
		%baseDisplace = vectorSub(%this.position, %basePos);
		%limit8x = mFloor(%size / 4) * 4;
		for (%x = 0; %x < %limit8x; %x += 4) {
			for (%y = 0; %y < %limit8x; %y += 4) {
				for (%z = 0; %z < %limit8x; %z += 4) {
					%displace = %x+1.5 SPC %y+1.5 SPC %z+1.5;
					%newPos = vectorAdd(%basePos, %displace);
					%this.bricks[%this.numBricks] = new fxDTSBrick() {
						datablock = "brick8xCubeDirtData";
						colorID = %this.colorId;
						colorFxID = 0;
						shapeFxID = 0;
						isPlanted = 0;
						position = %newPos;
						displace = vectorSub(%displace, %baseDisplace);
					};
					%this.numBricks += 1;
				}
			}
		}
		%limit4x = mFloor(%size / 2) * 2;
		for (%x = 0; %x < %limit4x; %x += 2) {
			for (%y = 0; %y < %limit4x; %y += 2) {
				for (%z = 0; %z < %limit4x; %z += 2) {
					if (%x >= %limit8x || %y >= %limit8x || %z >= %limit8x) {
						%displace = %x+0.5 SPC %y+0.5 SPC %z+0.5;
						%newPos = vectorAdd(%basePos, %displace);
						%this.bricks[%this.numBricks] = new fxDTSBrick() {
							datablock = "brick4xCubeDirtData";
							colorID = %this.colorId;
							colorFxID = 0;
							shapeFxID = 0;
							isPlanted = 0;
							position = %newPos;
							displace = vectorSub(%displace, %baseDisplace);
						};
						%this.numBricks += 1;
					}
				}
			}
		}
		for (%x = 0; %x < %size; %x++) {
			for (%y = 0; %y < %size; %y++) {
				for (%z = 0; %z < %size; %z++) {
					if (%x >= %limit4x || %y >= %limit4x || %z >= %limit4x) {
						%displace = %x SPC %y SPC %z;
						%newPos = vectorAdd(%basePos, %displace);
						%this.bricks[%this.numBricks] = new fxDTSBrick() {
							datablock = "brick2xCubeDirtData";
							colorID = %this.colorId;
							colorFxID = 0;
							shapeFxID = 0;
							isPlanted = 0;
							position = %newPos;
							displace = vectorSub(%displace, %baseDisplace);
						};
						%this.numBricks += 1;
					}
				}
			}
		}
	}
}

function TRT_ghostGroup::deleteBricks(%this) {
	for (%i = 0; %i < %this.numBricks; %i++)
		%this.bricks[%i].delete();
	%this.numBricks = 0;
}

function TRT_ghostGroup::delete(%this) {
	%this.deleteBricks();
	Parent::delete(%this);
}

function TRT_ghostGroup::setTransform(%this, %pos) {
	%this.position = %pos;
	%i = 0;
	for (%i = 0; %i < %this.numBricks; %i++) {
		%brick = %this.bricks[%i];
		%newPos = vectorAdd(%pos, %brick.displace);
		%brick.setTransform(%newPos);
	}
}
