datablock ProjectileData(TrenchDirtProjectile)
{
	shapeFile = "base/data/shapes/empty.dts";
	directDamage = 0;
	directDamageType = "";
	radiusDamageType = "";

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce = 0;
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	impactImpulse = 0;
	verticalImpulse = 0;
	explosion = "";
	particleEmitter = DirtShootEmitter;

	muzzleVelocity = 40;
	velInheritFactor = 1;

	armingDelay = 0;
	lifetime = 350;
	fadeDelay = 0;
	bounceElasticity = 0.5;
	bounceFriction = 0.20;
	isBallistic = true;
	gravityMod = 0;

	hasLight = false;
	lightRadius = 2;
	lightColor = "1 0.5 0";

	uiName = "Trench Dirt";
};

datablock ShapeBaseImageData(TrenchToolImage)
{
	shapeFile = "base/data/shapes/brickweapon.dts";
	mountPoint = 0;
	offset = "0 0 0";
	className = "WeaponImage";
	item = TrenchToolItem;
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

datablock ShapeBaseImageData(TrenchToolShovelImage : TrenchToolImage)
{
	doColorShift    = true;
	colorShiftColor = "0.48 0.56 0.48 1.00";
};

datablock ShapeBaseImageData(TrenchToolPlacerImage : TrenchToolImage)
{
	doColorShift    = true;
	colorShiftColor = "0.55 0.27 0.08 1.00";
	projectile = TrenchDirtProjectile;
	projectileType = Projectile;
};

datablock ItemData(TrenchToolItem)
{
	canDrop         = true;
	category        = "Weapon";
	className       = "Tool";
	doColorShift    = true;
	colorShiftColor = "0.55 0.27 0.08 1.00";
	image           = "TrenchToolImage";
	shapeFile       = "base/data/shapes/brickweapon.dts";
	uiName          = "Trench Tool";
};
