using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWeaponController
{
    [SerializeField] private List<EnemyWeaponDataListUnit> _weaponDataList;
    public List<EnemyWeaponDataListUnit> WeaponDataList => _weaponDataList;

    public void FlipWeaponsPlayerDetectors()
    {
        WeaponDataList.ForEach(weaponData => weaponData.WeaponGameObjectData.PlayerDetector.Flip());
    }
}

[System.Serializable]
public class EnemyWeaponDataListUnit
{
    [SerializeField] private EnemyWeaponUnit _weaponScriptableObject;
    public EnemyWeaponUnit WeaponScriptableObject => _weaponScriptableObject;

    [SerializeField] private EnemyWeaponGameObjectData _weaponGameObjectData;
    public EnemyWeaponGameObjectData WeaponGameObjectData => _weaponGameObjectData;   
}