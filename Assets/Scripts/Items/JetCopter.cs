using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetCopter : SpecialItem
{
    private const byte seconds = 30;
    public byte duration = seconds;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<JetCopter>();
    }

    public override void Use()
    {
        // Use the Jet Copter
        print("Tuco tuco tuco tuco tuco tuco...");
    }
}