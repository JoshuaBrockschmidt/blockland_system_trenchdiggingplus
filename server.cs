if(forceRequiredAddOn("Player_No_Jet") == $Error::AddOn_NotFound) {
	error("Player_No_Jet not found! Trench Digging Plus will not be loaded");
	return;
}

$TDP::filePath = filePath($Con::File) @ "/";
$TDP::serverPath = $TDP::filePath @ "server/";

exec($TDP::serverPath @ "core/compat.cs");
exec($TDP::serverPath @ "players/datablocks.cs");
exec($TDP::serverPath @ "tools/trenchtool/server.cs");
