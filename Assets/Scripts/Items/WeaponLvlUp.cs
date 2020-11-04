using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    _1st,
    Secondary
}

public class WeaponLvlUp : Item
{
    public WeaponType weaponType;
    public byte newLvl;
    public byte ammo;

    public override void GetItem(Penosa player)
    {
        if(weaponType == WeaponType._1st)
        {
            if(player.PlayerData._1stWeaponLevel != newLvl)
            {
                player.PlayerData._1stWeaponLevel = newLvl;            
                player.SetAmmo(WeaponType._1st, ammo);
            }
            else 
                player.SetAmmo(WeaponType._1st, ammo + player.PlayerData._1stWeaponAmmo);
        }
        else if(weaponType == WeaponType.Secondary) 
        {
            if(player.PlayerData._2ndWeaponLevel != newLvl)
            {
                player.PlayerData._2ndWeaponLevel = newLvl;
                player.SetAmmo(WeaponType.Secondary, ammo);
            }
            else
                player.SetAmmo(WeaponType.Secondary, player.PlayerData._2ndWeaponAmmo + ammo);
        }
    }
}