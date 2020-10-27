using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : SpecialItem
{
    private const byte adrenaline_duration = 30;
    public byte duration = adrenaline_duration;
    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<Adrenaline>();
    }

    public override void Use()
    {
        // Use Adrenaline
        print("Êta, carai!");
    }
}