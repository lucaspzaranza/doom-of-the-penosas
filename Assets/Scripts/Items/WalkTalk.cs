using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTalk : SpecialItem
{
    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<WalkTalk>();
    }

    public override void Use()
    {
        base.Use();
        // Use the Walk Talk
        print("Temptin' temptin' Louis Hampton...");
        RemoveItemIfAmountEqualsZero();
    }
}
