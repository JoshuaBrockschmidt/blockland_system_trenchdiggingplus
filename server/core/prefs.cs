////
// Handles preferences
////

function TDP_updateDirtGiving(%val) {
	if (!%val) {
		for (%i = 0; %i < clientGroup.getCount(); %i++) {
			%cl = clientGroup.getObject(%i);
			%to = %cl.TDP_give["offerTo"];
			if (isObject(%to)) {
				messageClient(%cl, '', "\c3Your dirt offer to\c6" SPC %to.name SPC "\c3has been cancelled due to a change in server preferences");
				messageClient(%to, '', "\c3The dirt offer from\c6" SPC %cl.name SPC "\c3has been cancelled due to a change in server preferences");
				%cl.TDP_endDirtOffer();
			}
		}
	}
}

function TDP_updateInfDirtForAll(%val) {
	if (%val)
		TDP_updateDirtGiving(false);
}

if(isObject(Glass) && Glass.serverLoaded) {
	registerPref("Trench Digging Plus", "General", "Max Dirt", "num", "TDP::maxDirt", "System_TrenchDiggingPlus", 500, "1 10000 0", "", 0, 0, 0);
	registerPref("Trench Digging Plus", "General", "Infinite Dirt", "bool", "TDP::infDirtForAll", "System_TrenchDiggingPlus", false, "", "TDP_updateInfDirtForAll", 0, 0, 0);
	registerPref("Trench Digging Plus", "General", "Can Give Dirt", "bool", "TDP::canGiveDirt", "System_TrenchDiggingPlus", true, "", "TDP_updateDirtGiving", 0, 0, 0);
	// TODO: check if BLG is compatible with colorset types yet
	registerPref("Trench Digging Plus", "General", "Default Dirt Color", "colorset", "TDP::defaultColor", "System_TrenchDiggingPlus", 0, "", "", 0, 0, 0);
}

// Maximum quantity of dirt a player can hold.
if ($TDP::maxDirt $= "")
	$TDP::maxDirt = 500;

// Whether all clients has infinite dirt.
if ($TDP::infDirtForAll $= "")
	$TDP::infDirtForAll = false;

// Default dirt color.
if ($TDP::defaultColor $= "")
	$TDP::defaultColor = 0;

// Whether client's can give each-other dirt.
if ($TDP::canGiveDirt $= "")
	$TDP::canGiveDirt = true;
