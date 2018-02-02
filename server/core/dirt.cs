////
// Functionality relating to a client's dirt inventory.
////

// Pushes dirt onto a client's dirt stack (LIFO). Does not account for
// maximum quantity of dirt a client can hold. As in, you are allowed to
// push dirt past the limit $TDP::maxDirt.
//
// @param GameConnection this	Target client.
// @param int num	        Quantity of dirt to push.
// @param int colorID		Color ID of dirt being added.
function GameConnection::TDP_pushDirt(%this, %num, %colorID) {
	for (%i = 0; %i < %num; %i++) {
		%this.TDP_dirt[%this.TDP_dirtCnt] = %colorID;
		%this.TDP_dirtCnt++;
	}
}

// Pulls dirt from a client's dirt stack.
//
// @param GameConnection this	Target client.
// @param int num	        Quantity of dirt to pull. If value exceeds
//				quantity of dirt in stack, whole stack will be
//				pulled.
// @return string		Color IDs of pulled dirt as a string of words
//				(left to right, LIFO).
function GameConnection::TDP_pullDirt(%this, %num) {
	%pullCnt = getMin(%num, %this.TDP_dirtCnt);
	%colorIDs = "";
	for (%i = 0; %i < %pullCnt; %i++)
		%colorIDs = %colorIDs SPC %this.TDP_dirt[%this.TDP_dirtCnt--];
	return trim(%colorIDs);
}

// Gets the color ID of a single dirt unit stored in a client's dirt stack.
//
// @param GameConnection this	Target client.
// @param int offset		Offset from top of client's dirt stack.
//				0 by default.
function GameConnection::TDP_getDirtColorID(%this, %offset) {
	%colorID = %this.TDP_dirt[%this.TDP_dirtCnt - %offset - 1];
	return %colorID;
}

// Sets how much dirt a client has. Added dirt is of the default color, as
// specified by the preference $TDP::defaultColor. Does not allow dirt to
// be set above the maximum dirt limit, $TDP::maxDirt, or below 0.
//
// @param GameConnection this	Client whose dirt quantity is to be set.
// @param number num		Number to set dirt quantity to.
function GameConnection::TDP_setDirt(%this, %num) {
	%num = mFloor(%num);
	if (%num > $TDP::maxDirt)
		%num = $TDP::maxDirt;
	else if (%num < 0)
		%num = 0;
	if (%num > %this.TDP_dirtCnt) {
		%pushCnt = %this.TDP_dirtCnt - %num;
		%this.TDP_pushDirt(%pushCnt);
	}
	%this.TDP_dirtCnt = %num;
}

// Adds to how much dirt a client has.
//
// @param GameConnection this	Client to add dirt to.
// @param number add		Quantity of dirt to add.
function GameConnection::TDP_addDirt(%this, %add) {
	%this.TDP_setDirt(%this.TDP_dirtCnt + %add);
}

// Subtracts from how much dirt a client has.
//
// @param GameConnection this	Client to subtract dirt from.
// @param number sub		Quantity of dirt to subtract.
function GameConnection::TDP_subDirt(%this, %sub) {
	%this.TDP_setDirt(%this.TDP_dirtCnt - %sub);
}

// Sets whether or not a client can place dig and place infinite quantities
// of dirt.
//
// @param GameConnection this	Target client.
// @param boolean bool		True if infinite dirt is to be enabled,
//				false if infinite dirt is to be disabled.
function GameConnection::TDP_setInfDirt(%this, %bool) {
	%this.TDP_isInfDirt = %bool;
}

// Sets whether or not a client can place dig and place without delay.
//
// @param GameConnection this	Target client.
// @param boolean bool		True if speed dirt is to be enabled,
//				false if speed dirt is to be disabled.
function GameConnection::TDP_setSpeedDirt(%this, %bool) {
        %this.TDP_isSpeedDirt = %bool;
}

// Toggles a client's ability to dig or place dirt without delay.
//
// @param GameConnection client	Client calling command.
// @param string n1 ... n6	Words of name of client being targeted.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdSpeedDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6) {
	if(%cl.isAdmin) {
		// Handle arguments.
		%name = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6);
		%target = (%name $= "") ? %cl : findClientByName(%name);

		// Set flag and inform caller and target of change.
	        %cl.TDP_setSpeedDirt(!%target.TDP_isSpeedDirt);
		%onOff = %target.TDP_isSpeedDirt ? "ON" : "OFF";
		messageClient(%cl, '' , "\c3Speed dirt is now\c6" SPC %onOff SPC "\c3for\c6" SPC %target.name);
		if (%target.getID() != %cl.getID())
			messageClient(%target, '', "\c3Speed dirt is now\c6" SPC %onOff SPC "\c3for you");
	}
	else {
		messageClient(%target, '', "\c3You must be admin to invoke this command");
	}
}

// Sets how much dirt a client has.
//
// @param GameConnection client	Client calling command.
// @param string n1 ... n7	Words of name of client being targeted, followed
//				by the quantity of dirt to set client's dirt to.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdSetDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	if (%cl.isAdmin) {
		// Handle arguments.
		%args = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6 SPC %n7);
		%cnt = getWordCount(%args);
		%name = trim(getWords(%args, 0, %cnt - 2));
		%newDirt = getWord(%args, %cnt - 1);
		%target = (%name $= "") ? %cl : findClientByName(%name);

		// Verify arguments.
		if (!isObject(%target)) {
			messageClient(%cl, '' , "\c3Player\c6" SPC %name SPC "\c3could not be found");
			return;
		}
		if (!strIsNum(%newDirt)) {
			messageClient(%target, '' , "\c3Invalid quantity of dirt");
			return;
		}

		// Set target's dirt.
		%target.TDP_setDirt(%newDirt);
		messageClient(%cl, '' , "\c6" @ %target.name SPC "\c3now has\c6" SPC %target.TDP_dirtCnt SPC "\c3dirt");
		if (%target.getID() != %cl.getID())
			messageClient(%target, '' , "\c6" @ %cl.name SPC "\c3set your dirt to\c6" SPC %target.TDP_dirtCnt SPC "\c3");
	}
	else {
		messageClient(%cl, '', "\c3You must be admin to invoke this command");
	}
}

// Adds to how much dirt a client has.
//
// @param GameConnection client	Client calling command.
// @param string n1 ... n7	Words of name of client being targeted, followed
//				by the quantity of dirt to add.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdAddDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	if(%cl.isAdmin) {
		// Handle arguments.
		%args = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6 SPC %n7);
		%cnt = getWordCount(%args);
		%name = trim(getWords(%args, 0, %cnt - 2));
		%addDirt = getWord(%args, %cnt - 1);
		%target = (%name $= "") ? %cl : findClientByName(%name);

		// Verify arguments.
		if (!isObject(%target)) {
			messageClient(%cl, '' , "\c3Player\c6" SPC %name SPC "\c3could not be found");
			return;
		}
		if (!strIsNum(%addDirt)) {
			messageClient(%target, '' , "\c3Invalid quantity of dirt to add");
			return;
		}

		// Add to target's dirt.
		%target.TDP_addDirt(%addDirt);
		messageClient(%cl, '' , "\c6" @ %target.name SPC "\c3now has\c6" SPC %target.TDP_dirtCnt SPC "\c3dirt");
		if (%target.getID() != %cl.getID())
			messageClient(%target, '' , "\c6" @ %cl.name SPC "\c3gave you\c6" SPC %target.TDP_dirtCnt SPC "\c3");
	}
	else {
		messageClient(%cl, '', "\c3You must be admin to invoke this command");
	}
}

// Subtracts from how much dirt a client has.
//
// @param GameConnection client	Client calling command.
// @param string n1 ... n7	Words of name of client being targeted, followed
//				by the quantity of dirt to subtract.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdSubDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	if(%cl.isAdmin) {
		// Handle arguments.
		%args = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6 SPC %n7);
		%cnt = getWordCount(%args);
		%name = trim(getWords(%args, 0, %cnt - 2));
		%subDirt = getWord(%args, %cnt - 1);
		%target = (%name $= "") ? %cl : findClientByName(%name);

		// Verify arguments.
		if (!isObject(%target)) {
			messageClient(%cl, '' , "\c3Player\c6" SPC %name SPC "\c3could not be found");
			return;
		}
		if (!strIsNum(%subDirt)) {
			messageClient(%target, '' , "\c3Invalid quantity of dirt to subtract");
			return;
		}

		// Subtract from target's dirt.
		%target.TDP_subDirt(%subDirt);
		messageClient(%cl, '' , "\c6" @ %target.name SPC "\c3now has\c6" SPC %target.TDP_dirtCnt SPC "\c3dirt");
		if (%target.getID() != %cl.getID())
			messageClient(%target, '' , "\c6" @ %cl.name SPC "\c3gave you\c6" SPC %target.TDP_dirtCnt SPC "\c3");
	}
	else {
		messageClient(%cl, '', "\c3You must be admin to invoke this command");
	}
}

// Toggles whether or not a client can place dig and place infinite quantities
// of dirt.
//
// @param GameConnection cl	Client calling command.
// @param string n1 ... n6	Words of name of client being targeted.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdInfDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6) {
	if(%cl.isAdmin) {
		%name = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6);
		%target = (%name $= "") ? %cl : findClientByName(%name);

		%target.TDP_setInfDirt(!%target.TDP_isInfDirt);
		%onOff = %target.TDP_isInfDirt ? "ON" : "OFF";
		messageClient(%cl, '' , "\c3Infinite dirt is now\c6" SPC %onOff SPC "\c3for\c6" SPC %target.name);
		if (%target.getID() != %cl.getID())
			messageClient(%target, '', "\c3Infinite dirt is now\c6" SPC %onOff SPC "\c3for you");
	}
	else {
		messageClient(%cl, '', "\c3You must be admin to invoke this command");
	}
}

// Terminates an offer to give a client dirt.
//
// @param GameConnection this   Client who is offering.
function GameConnection::TDP_endDirtOffer(%this) {
	%to = %this.TDP_give["offerTo"];
	if (isObject(%to))
		%to.TDP_give["offerFrom"] = -1;
	cancel(%this.TDP_give["offerEvent"]);
	%this.TDP_give["offerTo"] = -1;
}

// Prematurely cancels an offer to give a client dirt.
//
// @param GameConnection this   Client who is offering.
function GameConnection::TDP_timeoutDirtOffer(%this) {
	%to = %this.TDP_give["offerTo"];
	messageClient(%this, '', "\c3Your dirt offer to\c6" SPC %to.name SPC "\c3has timed out");
	messageClient(%to, '', "\c3The dirt offer from\c6" SPC %this.name SPC "\c3has timed out");
	%this.TDP_endDirtOffer();
}

// Offers dirt to another client. An offer to give dirt will only be made if
// the target client does not already have a pending request. Requests will
// only pend for two minutes.
//
// @param GameConnection cl	Client calling command.
// @param string n1 ... n7	Words of name of client being given dirt,
//				followed by the quantity of dirt to offer.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdGiveDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	if (%cl.TDP_isInfDirt || $TDP::infDirtForAll) {
		messageClient(%cl, '', "\c3You cannot give dirt when infinite dirt is enabled");
		return;
	}
	if (!$TDP::canGiveDirt) {
		messageClient(%cl, '', "\c3Dirt giving is disabled");
		return;
	}

	// Parse arguments.
	%args = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6 SPC %n7);
	%cnt = getWordCount(%args);
	%name = trim(getWords(%args, 0, %cnt - 2));
	%offer = getWord(%args, %cnt - 1);

	// Verify arguments.
	if (%name $= "" || %offer $= "") {
		messageClient(%cl, '' , "\c3Please provide a \c6username \c3and \c6number" SPC %name);
		return;
	}
	%target = findClientByName(%name);
	if (!isObject(%target)) {
		messageClient(%cl, '' , "\c3Player\c6" SPC %name SPC "\c3could not be found");
		return;
	}
	if (%target.getID() == %cl.getID()) {
		messageClient(%cl, '' , "\c3You cannot give yourself dirt");
		return;
	}
	if (%target.TDP_isInfDirt) {
		messageClient(%cl, '' , "\c3You cannot give dirt to someone with infinite dirt");
		return;
	}
	if (!strIsNum(%offer) || %offer < 1) {
		messageClient(%target, '' , "\c3Invalid quantity of dirt to give");
		return;
	}

	// Ensure player has dirt to give.
	if (%cl.TDP_dirtCnt < 1) {
		messageClient(%cl, '' , "\c3You have no dirt to give");
		return;
	}

	// Check if target client already has pending offer.
	if (isObject(%target.TDP_give["offerFrom"])) {
		messageClient(%cl, '', "\c6" @ %target.name SPC "\c3already has a pending offer");
		return;
	}

	// If client is already offering dirt, cancel their offer.
	if (isObject(%cl.TDP_give["offerTo"])) {
		messageClient(%cl, '', "\c3Your dirt offer to\c6" SPC %to.name SPC "\c3has been cancelled");
		messageClient(%to, '', "\c3The dirt offer from\c6" SPC %cl.name SPC "\c3has been cancelled");
		%cl.TDP_endDirtOffer();
	}

	// Ensure realistic dirt offer.
	if (%offer > %cl.TDP_dirtCnt)
		%offer = %cl.TDP_dirtCnt;
	else
		%offer = mCeil(%offer);

	// Give offer to target client.
	%cl.TDP_give["offerTo"] = %target;
	%cl.TDP_give["offer"] = %offer;
	%target.TDP_give["offerFrom"] = %cl;
	messageClient(%cl, '', "\c3You have offered\c6" SPC %offer SPC "\c3to\c6" SPC %target.name);
	messageClient(%cl, '', "\c3Type \c6/cancelDirt \c3to cancel this offer");
	messageClient(%target, '', "\c6" @ %cl.name SPC "\c3has offered you\c6" SPC %offer SPC "\c3dirt");
	messageClient(%target, '', "\c3Type \c6/acceptDirt \c3to accept this offer");
	messageClient(%target, '', "\c3Type \c6/rejectDirt \c3to reject this offer, or wait for it to timeout");

	// Cancel offer in two minutes if target client does not accept.
	%cl.TDP_give["offerEvent"] = %cl.schedule(120000, TDP_timeoutDirtOffer);
}

// Cancels a dirt offer.
//
// @param GameConnection cl	Client whose offer is being cancelled.
function serverCmdCancelDirt(%cl) {
	if (isObject(%cl.TDP_give["offerTo"])) {
		%to = %cl.TDP_give["offerTo"];
		messageClient(%cl, '', "\c3Your dirt offer to\c6" SPC %to.name SPC "\c3has been cancelled");
		messageClient(%to, '', "\c3The dirt offer from\c6" SPC %cl.name SPC "\c3has been cancelled");
		%cl.TDP_endDirtOffer();
	} else {
		messageClient(%cl, '', "\c3You are not currently offering any dirt");
	}
}

// Accepts a client's offer to give dirt.
//
// @param GameConnection cl	Client calling command.
function serverCmdAcceptDirt(%cl) {
	if (isObject(%cl.TDP_give["offerFrom"])) {
		%from = %cl.TDP_give["offerFrom"];
		%offer = %from.TDP_give["offer"];

		// Ensure realistic dirt offer.
		%max = getMin(%from.TDP_dirtCnt, $TDP::maxDirt - %cl.TDP_dirtCnt);
		%offer = getMin(%offer, %max);

		// Trade dirt.
		%colorIDs = %from.TDP_pullDirt(%offer);
		for (%i = %offer - 1; %i >= 0; %i--) {
			%colorID = getWord(%colorIDs, %i);
			%cl.TDP_pushDirt(1, %colorID);
		}

		messageClient(%from, '', "\c6" @ %cl.name SPC "\c3has accepted your offer of\c6" SPC %offer SPC "\c3dirt");
		messageClient(%cl, '', "\c3You have accepted\c6" SPC %offer SPC "\c3dirt from\c6" SPC %from.name);

		%from.TDP_endDirtOffer();
	}
	else {
		messageClient(%cl, '', "\c3Nobody is currently offering to give you dirt");
	}
}

// Reject a client's offer to give dirt.
//
// @param GameConnection cl	Client calling command.
function serverCmdRejectDirt(%cl) {
	%to = %cl.TDP_give["offerFrom"];
	if (isObject(%cl.TDP_give["offerFrom"])) {
		messageClient(%cl, '', "\c3Your dirt offer to\c6" SPC %to.name SPC "\c3has been rejected");
		messageClient(%to, '', "\c3The dirt offer from\c6" SPC %cl.name SPC "\c3has been rejected");
		%from.TDP_endDirtOffer();
	}
	else {
		messageClient(%cl, '', "\c3Nobody is currently offering to give you dirt");
	}
}

package TrenchDiggingPlusPackage {
	function GameConnection::onClientEnterGame(%this) {
		Parent::onClientEnterGame(%this);

		if (%this.TDP_dirtCnt $= "")
			%this.TDP_dirtCnt = 0;

		%this.TDP_give["offerFrom"] = -1;
		%this.TDP_give["offerTo"] = -1;
	}

	function GameConnection::onClientLeaveGame(%this) {
		// Cancel any pending dirt offers
		if (isObject(%this.TDP_give["offerTo"])) {
			%to = %this.TDP_give["offerTo"];
			messageClient(%to, '', "\c3The dirt offer from\c6" SPC %this.name SPC "\c3has been cancelled");
			%this.TDP_endDirtOffer();
		}
		if (isObject(%this.TDP_give["offerFrom"])) {
			%from = %this.TDP_give["offerFrom"];
			messageClient(%from, '', "\c3Your dirt offer to\c6" SPC %this.name SPC "\c3has been cancelled");
			%from.TDP_endDirtOffer();
		}

		Parent::onClientLeaveGame(%this);
	}
};
