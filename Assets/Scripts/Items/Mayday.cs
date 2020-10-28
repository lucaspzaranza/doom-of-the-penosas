using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mayday : SpecialItem
{
    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<Mayday>();
    }

    public override void Use()
    {
        // Resources.Find pra pegar o prefab do avião may day
        // Use Mayday
        print("Mayday, mayday!");
    }
}
