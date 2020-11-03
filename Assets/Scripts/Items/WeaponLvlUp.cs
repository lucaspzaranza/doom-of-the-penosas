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
            if(player.PrimaryWeaponLevel != newLvl)
            {
                player.PrimaryWeaponLevel = newLvl;            
                player.SetAmmo(WeaponType.Primary, ammo);
            }
            else 
                player.SetAmmo(WeaponType.Primary, ammo + player.PrimaryWeaponAmmo);
        }
        else if(weaponType == WeaponType.Secondary) 
        {
            if(player.SecondaryWeaponLevel != newLvl)
            {
                player.SecondaryWeaponLevel = newLvl;
                player.SetAmmo(WeaponType.Secondary, ammo);
            }
            else
                player.SetAmmo(WeaponType.Secondary, player.SecondaryWeaponAmmo + ammo);
        }
    }
}