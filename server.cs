if(ForceRequiredAddOn("Gamemode_TrenchDigging") == $Error::AddOn_NotFound) {
	error("Gamemode_TrenchDigging not found! Better Trench Tool will not be loaded");
	return;
}

$BTT::FilePath = filePath($Con::File) @ "/";
$BTT::ScriptsPath = $BTT::FilePath @ "scripts/";

if(isObject(BTT_ServerGroup))
	BTT_ServerGroup.delete();
new ScriptGroup(BTT_ServerGroup);

echo("--- Loading Better Trench Tool config ---");
exec($BTT::FilePath @ "config.cs");
// Validate config and modify if necessary
if ($BTT::MaxCubeSizeBricks < 1)
	$BTT::MaxCubeSizeBricks = 1;
else
	$BTT::MaxCubeSizeBricks = mFloor($BTT::MaxCubeSizeBricks);
if ($BTT::MaxCubeSizeCubes < 1)
	$BTT::MaxCubeSizeCubes = 1;
else
	$BTT::MaxCubeSizeCubes = mFloor($BTT::MaxCubeSizeCubes);

// Other global variables

// Possible tool modes
$BTT::DisabledMode = 0;
$BTT::ShovelMode   = 1;
$BTT::PlacerMode   = 2;

echo("--- Loading Better Trench Tool server scripts ---");
exec($BTT::ScriptsPath @ "bettertrenchtool.cs");
exec($BTT::ScriptsPath @ "bettertrenchtoolimage.cs");
exec($BTT::ScriptsPath @ "datablocks.cs");
exec($BTT::ScriptsPath @ "ghostgroup.cs");
exec($BTT::ScriptsPath @ "other.cs");
exec($BTT::ScriptsPath @ "placermode.cs");
exec($BTT::ScriptsPath @ "shovelmode.cs");
exec($BTT::ScriptsPath @ "toolmode.cs");

echo("--- Activating Better Trench Tool package ---");
activatePackage(BetterTrenchToolPackage);
