%delayMult = 1;

datablock ItemData(BetterTrenchToolItem)
{
	canDrop         = true;
	category        = "Weapon";
	className       = "Tool";
	doColorShift    = true;
	colorShiftColor = "1.00 0.84 0.00 1.00";
	image           = "BetterTrenchToolImage";
	shapeFile       = "base/data/shapes/brickweapon.dts";
	uiName          = "Better Trench Tool";
};

datablock ShapeBaseImageData(BetterTrenchToolImage)
{
	shapeFile = "base/data/shapes/brickweapon.dts";
	mountPoint = 0;
	offset = "0 0 0";
	className = "WeaponImage";
	item = BetterTrenchToolItem;
	ammo = " ";
	projectile = TrenchDirtProjectile;
	projectileType = Projectile;
	correctMuzzleVector = false;
	melee = false;
	doRetraction = false;
	armReady = true;

	stateName[0]                    = "Activate";
	stateTransitionOnTimeout[0]     = "Ready";
	stateTimeoutValue[0]            = 0.1;
	stateSound[0]                   = weaponSwitchSound;
};

datablock ShapeBaseImageData(BetterTrenchToolShovel1xImage : BetterTrenchToolImage)
{
	melee = false;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.48 0.56 0.48 1.00";

	stateName[0]                    = "Activate";
	stateTransitionOnTimeout[0]     = "Ready";
	stateTimeoutValue[0]            = 0.5;
	stateSound[0]                   = 0;

	stateName[1]                    = "Ready";
	stateTransitionOnTriggerDown[1] = "PreFire";
	stateAllowImageChange[1]        = true;

	stateName[2]                    = "PreFire";
	stateScript[2]                  = "onPreFire";
	stateTransitionOnTimeout[2]     = "Fire";
	stateAllowImageChange[2]        = true;
	stateTimeoutValue[2]            = 0.1;

	stateName[3]                    = "Fire";
	stateScript[3]                  = "onFire";
	stateAllowImageChange[3]        = false;
	stateTimeoutValue[3]            = 0.3;
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateWaitForTimeout[3]          = true;
	stateFire[3]                    = true;

	stateName[4]                    = "CheckFire";
	stateTransitionOnTriggerUp[4]   = "StopFire";
	stateTransitionOnTriggerDown[4] = "PreFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateAllowImageChange[5]        = false;
	stateTimeoutValue[5]            = 0.2;
	stateWaitForTimeout[5]          = true;
};

datablock ShapeBaseImageData(BetterTrenchToolShovel2xImage : BetterTrenchToolShovel1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult;
};

datablock ShapeBaseImageData(BetterTrenchToolShovel3xImage : BetterTrenchToolShovel1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 2;
};

datablock ShapeBaseImageData(BetterTrenchToolShovel4xImage : BetterTrenchToolShovel1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 3;
};

datablock ShapeBaseImageData(BetterTrenchToolPlacer1xImage : BetterTrenchToolShovel1xImage)
{
	projectile = TrenchDirtProjectile;
	projectileType = Projectile;
	colorShiftColor = "0.55 0.27 0.08 1.00";
};

datablock ShapeBaseImageData(BetterTrenchToolPlacer2xImage : BetterTrenchToolPlacer1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult;
};

datablock ShapeBaseImageData(BetterTrenchToolPlacer3xImage : BetterTrenchToolPlacer1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 2;
};

datablock ShapeBaseImageData(BetterTrenchToolPlacer4xImage : BetterTrenchToolPlacer1xImage)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 3;
};
