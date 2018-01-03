if (forceRequiredAddOn("Player_No_Jet") == $Error::AddOn_NotFound) {
	error("Player_No_Jet not found! Trench Digging Plus will not be loaded");
	return;
}

if (forceRequiredAddOn("Brick_Large_Cubes") == $Error::AddOn_NotFound) {
	error("Brick_Large_Cubes not found! Trench Digging Plus will not be loaded");
	return;
}

$TDP::filePath = filePath($Con::File) @ "/";
$TDP::serverPath = $TDP::filePath @ "server/";

exec($TDP::serverPath @ "bricks/datablocks.cs");
exec($TDP::serverPath @ "players/datablocks.cs");
exec($TDP::serverPath @ "tools/trenchtool/server.cs");
