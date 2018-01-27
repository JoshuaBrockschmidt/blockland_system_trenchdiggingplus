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

// Gives dirt to another client. A request to give dirt will only be made if
// the target client does not already have a pending request. Requests will
// only pend for two minutes.
//
// @param GameConnection client	Client calling command.
// @param string n1 ... n7	Words of name of client being given dirt,
//				followed by the quantity of dirt to give.
//				In absence of a name, the client who called the
//				command will be targeted.
function serverCmdGiveDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	// Check if target client already has pending request.
	// TODO

	// Give request to target client.
	// TODO

	// Cancel request in two mintutes if target client does not accept.
	// TODO
}

// Accepts a client's offer to give dirt.
//
// @param GameConnection client	Client calling command.
function serverCmdAcceptDirt(%cl, %n1, %n2, %n3, %n4, %n5, %n6, %n7) {
	// TODO
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

package TrenchDiggingPlusPackage {
	function GameConnection::onClientEnterGame(%this) {
		Parent::onClientEnterGame(%this);

		if (%this.TDP_dirtCnt $= "")
			%this.TDP_dirtCnt = 0;
	}
};
