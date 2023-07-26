using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    public byte amount;

    public override void GetItem(Penosa player)
    {
        if(!player.Adrenaline || (player.Adrenaline && player.PlayerData.Life > 0))
            player.PlayerData.Life += amount;        
    }
}