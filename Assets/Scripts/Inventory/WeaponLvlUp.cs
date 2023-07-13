using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Primary,
    Secondary
}

public class WeaponLvlUp : Item
{
    public WeaponType weaponType;
    public byte newLvl;
    public byte ammo;

    public override void GetItem(Penosa player)
    {
        if(weaponType == WeaponType.Primary)
        {
            if(player.PlayerData._1stWeaponLevel != newLvl)
            {
                player.PlayerData._1stWeaponLevel = newLvl;            
                player.SetAmmo(WeaponType.Primary, ammo);
            }
            else 
                player.SetAmmo(WeaponType.Primary, ammo + player.PlayerData._1stWeaponAmmoProp);
        }
        else if(weaponType == WeaponType.Secondary) 
        {
            if(player.PlayerData._2ndWeaponLevel != newLvl)
            {
                player.PlayerData._2ndWeaponLevel = newLvl;
                player.SetAmmo(WeaponType.Secondary, ammo);
            }
            else
                player.SetAmmo(WeaponType.Secondary, player.PlayerData._2ndWeaponAmmoProp + ammo);
        }
    }
}