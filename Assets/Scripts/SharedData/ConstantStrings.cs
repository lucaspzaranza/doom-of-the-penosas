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

    // Messages
    public const string ReachedMaxTimeAttempingToGetTheParentController =
        "Reached the max time duration attempting to get the parent controller. Aborting operation.";
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