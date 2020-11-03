using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public override void GetItem(Penosa player)
    {     
        player.ArmorLife = Penosa.max_life;
    }
}