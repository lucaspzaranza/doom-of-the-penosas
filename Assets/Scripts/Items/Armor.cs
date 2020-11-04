using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public override void GetItem(Penosa player)
    {     
        player.PlayerData.ArmorLife = PlayerConsts.max_life;
    }
}