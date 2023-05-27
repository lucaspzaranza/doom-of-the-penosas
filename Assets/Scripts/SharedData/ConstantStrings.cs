using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstantStrings
{
    // Projectiles
    public const string EggShot = "Egg Shot";
    public const string BigEggShot = "Big Egg Shot";
    public const string Grenade = "Grenade";
    public const string Shuriken = "Shuriken";
    public const string FuumaShuriken = "Fuuma Shuriken";

    // Player
    public const string PlayerTag = "Player";

    // Animator
    public const string Shoot = "Shoot";
    public const string ShotLevel = "Shot Level";
    public const string XVelocity = "XVelocity";
    public const string YVelocity = "YVelocity";
    public const string IsGrounded = "IsGrounded";
    public const string Up = "Up";
    public const string Down = "Down";
    public const string JetCopter = "JetCopter";
    public const string TurnOff = "TurnOff";

    // Menu
    public const string CursorPositionName = "CursorPosition";
}

public static class WarningMessages
{
    public static void ControllerNotFoundOnBackupMessage(string controllerName)
    {
        Debug.LogWarning($"The {controllerName} could not be found. Are you missing the Controller instance on the scene?");
    }

    public static void CantAddPlayer(string playerName)
    {
        Debug.LogWarning($"Can't add {playerName} to the game. Please check if there is an appropriate input device inserted on your machine.");
    }
}

public static class InputStrings
{
    public const string Jump = "Jump";
    public const string Fire1 = "Fire1";
    public const string Fire2 = "Fire2";
    public const string Fire3 = "Fire3";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string ChangeSpecialItem = "ChangeSpecialItem";
    public const string Start = "Start";
}

public static class ConstantNumbers
{
    // Player
    public const int NumberOfPlayers = 2;
    public const int CountdownSeconds = 10;
}   

public static class ScenesBuildIndexes
{
    public const int MainMenu = 0;
    public const int MapaMundi = 1;
    public const int _1stStage = 2;
}

public static class PlayerConsts
{
    public const float MachineGunInterval = 0.1f;
    public const float ShotLvl3Duration = 0.5f;
    public const float ShotLvl2VariationRate = 0.1f;
    public const float ShotAnimDuration = 0.25f;
    public const float DefaultSpeed = 2f;
    public const float OverlapCircleDiameter = 0.1f;
    public const float BlinkInitialValue = 0.1f;
    public const byte Max_Life = 100;
    public const byte Max_Lives = 9;
    public const byte Countdown = 10;
    public const byte Continues = 5;
    public const byte Initial_Lives = 5;
    public const byte WeaponInitialLevel = 1;
    public const byte _1stWeaponMaxLevel = 3;
    public const byte _2ndWeaponMaxLevel = 2;
    public const byte _1stWeaponMaxLvl = 3;
    public const byte _2ndWeaponMaxLvl = 2;
    public const byte _1stWeaponInitialAmmo = 0;
    public const byte _2ndWeaponInitialAmmo = 10;
    public const byte ArmorInitialLife = 10;
    public const int MaxAmmo = 999;
    public const int DeathLife = 0;
    public const int GameOverLives = 0;
    public const int InputZeroValue = 0;

    public const string Fire1Action = "Fire1";
    public const string Fire2Action = "Fire2";
    public const string Fire3SpecialAction = "Fire3Special";
    public const string MoveAction = "MoveAndAim";
    public const string ParachuteAction = "Parachute";
    public const string JumpAction = "Jump";
    public const string ChangeSpecialItemAction = "ChangeSpecialItem";
}