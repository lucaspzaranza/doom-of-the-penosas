using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyWeaponController
{
    [SerializeField] private List<EnemyWeaponDataListUnit> _weaponDataList;
    public List<EnemyWeaponDataListUnit> WeaponDataList => _weaponDataList;

    public void FlipWeaponsPlayerDetectors()
    {
        WeaponDataList.ForEach(weaponData => {
            bool invertOverlap = !weaponData.WeaponGameObjectData.FireInVerticalAxis;

            weaponData.WeaponGameObjectData.Flip();
            if (weaponData.WeaponGameObjectData.UseRotationLimit)
                weaponData.WeaponGameObjectData.UpdateRotationLimits();
        });
    }

    public int GetClosestWeaponIndexFrom(Vector2 playerPosition)
    {
        int index = 0;

        if(WeaponDataList.Count > 1)
        {
            float previousDistance = 
                Vector2.Distance(WeaponDataList[0].WeaponGameObjectData.transform.position, playerPosition);

            for (int i = 1; i < WeaponDataList.Count; i++)
            {
                float currentDistance = 
                    Vector2.Distance(WeaponDataList[i].WeaponGameObjectData.transform.position, playerPosition);
                if (currentDistance < previousDistance)
                {
                    index = 1;
                    previousDistance = currentDistance;
                }
            }
        }

        return index;
    }

    public bool HasWeaponWhichRotates()
    {
        return WeaponDataList.Any(weapon => weapon.WeaponGameObjectData.RotateTowardsPlayer);
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