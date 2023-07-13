using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLives : Item
{
    public byte amount;

    public override void GetItem(Penosa player)
    {
        player.PlayerData.Lives += amount;
    }
}