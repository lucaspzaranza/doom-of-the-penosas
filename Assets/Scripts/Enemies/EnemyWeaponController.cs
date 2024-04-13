using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWeaponController
{
    [SerializeField] private List<EnemyWeaponUnit> _weaponUnits;
    public List<EnemyWeaponUnit> WeaponUnits => _weaponUnits;
}
