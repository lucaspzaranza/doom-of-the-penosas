using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using UnityEngine;

public static class ConstantStrings
{
    // Projectiles
    public const string EggShot = "Egg Shot";
    public const string BigEggShot = "Big Egg Shot";
    public const string Grenade = "Grenade";
    public const string Shuriken = "Shuriken";
    public const string FuumaShuriken = "Fuuma Shuriken";
    public const string YarnShot = "Yarn shot";

    // Player
    public const string RideArmorTag = "RideArmor";
    public const string RideArmorCoreTag = "RideArmorCore";

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
    public const string NewGameSubtitle = "New Game";
    public const string ContinueGameSubtitle = "Continue";
    public const string HUD = "HUD";

    // Devices
    public const string Mouse = "Mouse";
    public const string Keyboard = "Keyboard";
    public const string Joystick = "Joystick";

    // Tags
    public const string PlayerTag = "Player";
    public const string CharacterSelectionButtonTag = "CharacterSelectionButton";
    public const string DeviceSelectionButtonTag = "DeviceSelectionButton";
    public const string EnemyProjectileTag = "Enemy Projectile";

    // Menu Messages
    public const string HUDGameOver = "Wait until the stage is over.";
    public const string HUDNoMoreContinues = "No more continues.";

    // Lobby
    public const string SelectCharacterMsg = "Select your character to play.";
    public const string SelectTheDeviceAndSeeTheControls = "Select the device you wish to use with this player." +
                                                "\nPress confirm to see the device controls.";

    // Persistence
    public const string CompletedStagesKey = "CompletedStages";
}

public static class WarningMessages
{
    public static void ControllerNotFoundOnBackupMessage(string controllerName)
    {
        Debug.LogWarning($"The {controllerName} could not be found. Are you missing the Controller instance on the scene?");
    }

    public static void CantAddPlayerMessage(string playerName)
    {
        Debug.LogWarning($"Can't add {playerName} to the game. Please check if there is an appropriate input device inserted on your machine.");
    }

    public static string InputDeviceChosenTwiceOrMoreMessage(string device)
    {
        string message = $"The {device} device is being chosen by more than one player. Please select another device.";
        Debug.LogWarning(message);
        return message;
    }

    public static void ButtonComponentNotFound()
    {
        Debug.LogWarning($"The GameObject hasn't any Button Component, and he may not behave properly.");
    }

    public static void PlayerDataAlreadyExists(string characterToAdd, int idToAdd)
    {
        Debug.LogWarning($"Player data for {characterToAdd} with ID {idToAdd} already exists.");
    }

    public static void SavingProgressFromTheBeggining()
    {
        Debug.Log("Saving game progress from the beginning...");
    }

    public static void EnemyActionNotFound(EnemyState enemyState)
    {
        Debug.LogWarning($"Could not find a proper action to perform for the {enemyState} enemy state. " +
            $"Did you add it at the Inspector correctly?");
    }

    public static void EnemyWeaponIsInactive(string enemyName, string weaponName)
    {
        Debug.LogWarning($"{enemyName} could not shoot with {weaponName} because the weapon is inactive. " +
            $"Did you forget to change to an active weapon at the Inspector?");
    }

    public static void PlayerDetectorNotFound()
    {
        Debug.LogWarning("Player Detector not found. You must add this component to make the rotation detection.");
    }

    public static void EnemyWeaponUnitCouldNotFoundGameControllerInstance()
    {
        Debug.LogWarning("Could not find GameController instance, " +
                    "so the pooling won't work as expected.");
    }

    public static void BossHasNoAttackWaves()
    {
        Debug.LogWarning("Boss has no attack waves. Did you forgot to setup some waves for it?");
    }

    public static void BossHasNoCurrentWaveSelected()
    {
        Debug.LogWarning("Boss has no current wave selected. Please select one from the attack waves list.");
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
    public const float TimeToReactivatePlayerColliderAfterFallingFromPlatform = 0.3f;

    // Stage
    public const float TimeToShowStageClearTxt = 2f;
    public const float TimeToReturnToMapaMundiAfterGameOver = 3f;

    // Enemy
    public const float UpsideDownAngle = 180f;
}   

public static class ScenesBuildIndexes
{
    public const int  MainMenu = 0;
    public const int MapaMundi = 1;
    public const int _1stStage = 2;
    public const int _2ndStage = 3;
    public const int _3rdStage = 4;
    public const int _4thStage = 5;
    public const int _5thStage = 6;
    public const int _6thStage = 7;
    public const int   EndGame = 8;
    public const int  CutScene = 9;
}

public static class PlayerConsts
{
    public const float MachineGunInterval = 0.1f;
    public const float RideArmorMachineGunInterval = 0.075f;
    public const float ShotLvl3Duration = 0.5f;
    public const float ShotLvl2VariationRate = 0.1f;
    public const float ShotAnimDuration = 0.25f;
    public const float DefaultSpeed = 2f;
    public const float OverlapCircleDiameter = 0.1f;
    public const float ChickencopterOverlapCircleDiameter = 0.7f;
    public const float BlinkInitialValue = 0.1f;
    public const float DefaultGravity = 1.5f;
    public const byte Max_Life = 100;
    public const byte Max_Lives = 9;
    public const byte Continues = 5;
    public const byte Initial_Lives = 5;
    public const byte WeaponInitialLevel = 1;
    public const byte _1stWeaponMaxLevel = 3;
    public const byte _2ndWeaponMaxLevel = 2;
    public const byte _1stWeaponMaxLvl = 3;
    public const byte _2ndWeaponMaxLvl = 2;
    public const byte _1stWeaponInitialAmmo = 0;
    public const byte _2ndWeaponInitialAmmo = 10;
    public const byte ArmorInitialLife = 0;
    public const int MaxAmmo = 999;
    public const int DeathLife = 0;
    public const int GameOverLives = 0;
    public const int InputZeroValue = 0;

    public const string Fire1Action = "Fire1";
    public const string Fire2Action = "Fire2";
    public const string Fire3SpecialAction = "Fire3Special";
    public const string MoveAction = "MoveAndAim";
    public const string PauseMenu = "PauseMenu";
    public const string ParachuteAction = "Parachute";
    public const string JumpAction = "Jump";
    public const string ChangeSpecialItemAction = "ChangeSpecialItem";
}