datablock ItemData(BetterTrenchToolItem)
{
	canDrop         = true;
	category        = "Weapon";
	className       = "Tool";
	doColorShift    = true;
	colorShiftColor = "0.55 0.27 0.08 1.00";
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
	ammo = 1;
	melee = false;
	doRetraction = false;
	armReady = true;

	stateName[0]                    = "Activate";
	stateTransitionOnTimeout[0]     = "Ready";
	stateTimeoutValue[0]            = 0.1;
	stateSound[0]                   = weaponSwitchSound;

	stateName[1]                    = "Ready";
	stateScript[2]                  = "onReady";
	stateTransitionOnTriggerDown[1] = "PreFire";
	stateAllowImageChange[1]        = true;

	stateName[2]                    = "PreFire";
	stateScript[2]                  = "onPreFire";
	stateTransitionOnTimeout[2]     = "CheckWait";
	stateAllowImageChange[2]        = true;
	stateTimeoutValue[2]            = 0.1;

	stateName[3]                    = "CheckWait";
	stateTransitionOnAmmo[3]        = "Fire";
	stateTransitionOnNoAmmo[3]	= "Wait";
	stateAllowImageChange[3]        = true;

	stateName[4]                    = "Wait";
	stateTransitionOnTimeout[4]     = "CheckWait";
	stateAllowImageChange[4]        = true;
	stateTimeoutValue[4]            = 0.1;

	stateName[5]                    = "Fire";
	stateScript[5]                  = "onFire";
	stateAllowImageChange[5]        = false;
	stateTimeoutValue[5]            = 0.3;
	stateTransitionOnTimeout[5]     = "CheckFire";
	stateFire[5]                    = true;

	stateName[6]                    = "CheckFire";
	stateTransitionOnTriggerUp[6]   = "StopFire";
	stateTransitionOnTriggerDown[6] = "PreFire";

	stateName[7]                    = "StopFire";
	stateTransitionOnTimeout[7]     = "Ready";
	stateAllowImageChange[7]        = false;
	stateTimeoutValue[7]            = 0.2;
};

datablock ShapeBaseImageData(BetterTrenchToolShovelImage : BetterTrenchToolImage)
{
	doColorShift    = true;
	colorShiftColor = "0.48 0.56 0.48 1.00";
};

datablock ShapeBaseImageData(BetterTrenchToolPlacerImage : BetterTrenchToolImage)
{
	doColorShift    = true;
	colorShiftColor = "0.55 0.27 0.08 1.00";
	projectile = TrenchDirtProjectile;
	projectileType = Projectile;
};
