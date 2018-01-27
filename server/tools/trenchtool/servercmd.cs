////
// Relevant server commands.
////

// Displays information on how to use the Trench Tool.
// Contains a Description and Controls section.
//
// @param section	Section number. 1 for Description and 2 for Controls
function serverCmdTTHelp(%cl, %section) {
	%bullet = "  <font:impact:17>\c9*  <font:palatino linotype:25>";
	%cl.chatMessage(" ");
	%cl.chatMessage("<rmargin:400><just:center><font:Impact:40><shadow:2:2>\c6Trench Tool Help");
	%cl.chatMessage("<rmargin:400><just:center><font:Impact:20><shadow:2:2>\c6---------------------------------------------------------------------");
	switch (%section) {
	case 1:
		%cl.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Description:<br>");
		%cl.chatMessage("<rmargin:1000><just:left>");
		%cl.chatMessage(%bullet SPC "\c6The \c4Trench Tool \c6works as a dirt placer and shovel.");
		%cl.chatMessage(%bullet SPC "\c6It also allows you to adjust how much dirt you can dig and place at a time.");
	case 2:
		%cl.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Controls:<br>");
		%cl.chatMessage("<rmargin:1000><just:left>");
		%cl.chatMessage(%bullet SPC "\c6Use the <Light Key> \c6to switch between modes.");
		%cl.chatMessage(%bullet SPC "\c3Shovel Mode \c6is for digging trench dirt.");
		%cl.chatMessage(%bullet SPC "\c3Placer Mode \c6is for placing trench dirt.");
		%cl.chatMessage(%bullet SPC "\c6Use <Shift Brick Up> \c6to increase the amount of dirt you can place and dig.");
		%cl.chatMessage(%bullet SPC "\c6Use <Shift Brick Down> \c6to decrease the amount of dirt you can place and dig.");
		%cl.chatMessage(%bullet SPC "\c6Use <Super Shift> \c6to quickly change between the smallest and biggest cube size.");
		%cl.chatMessage(%bullet SPC "\c6Change your paint color to change the color of the dirt you place.");
		%cl.chatMessage("      \c6(if the host has enabled this option)");
	default:
		%cl.chatMessage("<rmargin:400><just:center><font:Impact:30>\c6Help Sections:<br>");
		%cl.chatMessage("<rmargin:1000><just:left>");
		%cl.chatMessage("  \c61 - \c3Description");
		%cl.chatMessage("  \c62 - \c3Controls");
	}
}

// TODO: should this even be implemented?
//
// @param GameConnection cl	Target client.
function serverCmdDumpDirt(%cl) {
	if (%cl.TRT_mode.getName() !$= TRT_DisabledMode) {
		return; // TODO
	}
}
