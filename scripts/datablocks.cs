%delayMult = 1;

datablock ItemData(BetterTrenchToolItem)
{
	canDrop         = true;
	category        = "Weapon";
	className       = "Tool";
	doColorShift    = true;
	colorShiftColor = "1 0.84 0 1";
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
	doColorShift = true;
	colorShiftColor = BetterTrenchToolItem.colorShiftColor;

	stateName[0]                    = "Activate";
	stateTransitionOnTimeout[0]     = "Ready";
	stateTimeoutValue[0]            = 0.1;
	stateSound[0]                   = weaponSwitchSound;
};

datablock ShapeBaseImageData(BetterTrenchToolImage_1x : BetterTrenchToolImage)
{
	projectile = TrenchDirtProjectile;
	projectileType = Projectile;
	melee = false;
	doRetraction = false;
	armReady = true;

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
	stateAllowImageChange[2]        = false;
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

datablock ShapeBaseImageData(BetterTrenchToolImage_2x : BetterTrenchToolImage_1x)
{
	stateTimeoutValue[2] = 0.1 + %delayMult;
	item = BetterTrenchToolItem_2x;
};

datablock ShapeBaseImageData(BetterTrenchToolImage_3x : BetterTrenchToolImage_1x)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 2;
	item = BetterTrenchToolItem_3x;
};

datablock ShapeBaseImageData(BetterTrenchToolImage_4x : BetterTrenchToolImage_1x)
{
	stateTimeoutValue[2] = 0.1 + %delayMult * 3;
	item = BetterTrenchToolItem_4x;
};
