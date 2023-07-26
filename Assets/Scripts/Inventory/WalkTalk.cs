using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTalk : SpecialItem
{
    public static Action<byte> OnWalkTalk;

    public override void GetPlayerValues()
    {
        
    }

    public override void Use()
    {
        base.Use();
        OnWalkTalk?.Invoke(Player.PlayerData.LocalID);
        RemoveItemIfAmountEqualsZero();
    }
}
