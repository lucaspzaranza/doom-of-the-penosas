using System;
using UnityEngine;

public static class PlayerEvents
{
    public static Func<Vector2> GetPlayerStartPosition;
    public static Func<Transform> GetFirstPlayerTransform;
}