using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTalk : SpecialItem
{
    public override void GetPlayerValues()
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        base.Use();
        // Use the Walk Talk
        print("Temptin' temptin' Louis Hampton...");
        RemoveItemIfAmountEqualsZero();
    }
}
