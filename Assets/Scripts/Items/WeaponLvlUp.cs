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
    private const int _1stWeaponLv2Ammo = 250;
    private const int _1stWeaponLv3Ammo = 30;
    private const int _2ndWeaponLv2Ammo = 10;
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
                player.SetAmmo(WeaponType.Primary, newLvl <= 2? _1stWeaponLv2Ammo : _1stWeaponLv3Ammo);
            }
            else player.SetAmmo(WeaponType.Primary, (newLvl <= 2?
                _1stWeaponLv2Ammo : _1stWeaponLv3Ammo) + player.PrimaryWeaponAmmo);
        }
        else if(weaponType == WeaponType.Secondary) 
        {
            if(player.SecondaryWeaponLevel != newLvl)
            {
                player.SecondaryWeaponLevel = newLvl;
                player.SetAmmo(WeaponType.Secondary, _2ndWeaponLv2Ammo);
            }
            else player.SetAmmo(WeaponType.Secondary, player.SecondaryWeaponAmmo + _2ndWeaponLv2Ammo);
        }
    }
}