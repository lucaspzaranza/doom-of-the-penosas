using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWeaponController
{
    [SerializeField] private List<EnemyWeaponData> _weaponDataList;
    public List<EnemyWeaponData> WeaponDataList => _weaponDataList;    
}

[System.Serializable]
public class EnemyWeaponData
{
    [SerializeField] private EnemyWeaponUnit _weaponUnit;
    public EnemyWeaponUnit WeaponUnit => _weaponUnit;

    [SerializeField] private EnemyWeaponSpawnTransform _enemyWeaponSpawnTransform;
    public EnemyWeaponSpawnTransform EnemyWeaponSpawnTransform => _enemyWeaponSpawnTransform;
}