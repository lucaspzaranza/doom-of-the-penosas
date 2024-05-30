using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWeaponController
{
    [SerializeField] private List<EnemyWeaponDataListUnit> _weaponDataList;
    public List<EnemyWeaponDataListUnit> WeaponDataList => _weaponDataList;    
}

[System.Serializable]
public class EnemyWeaponDataListUnit
{
    [SerializeField] private EnemyWeaponUnit _weaponUnit;
    public EnemyWeaponUnit WeaponUnit => _weaponUnit;

    [SerializeField] private EnemyWeaponGameObjectData _gameObjectData;
    public EnemyWeaponGameObjectData GameObjectData => _gameObjectData;
}