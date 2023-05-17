using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantStrings : MonoBehaviour
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
    public const string ArrowPositionName = "ArrowPosition";
}

public class InputStrings
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

public class ConstantNumbers
{
    public const int NumberOfPlayers = 2;
    public const int CountdownSeconds = 10;
}
