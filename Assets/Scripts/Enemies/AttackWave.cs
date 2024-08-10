using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackWaveWeaponData
{
    [Tooltip("Check this if you want to overwrite the current weapon data with new data. " +
    "It'll be useful if you want to use the same weapon to fire different types of projectiles, or " +
    "if you want to change the weapon fire pattern from a single to a continuous, for example.")]
    [SerializeField] private bool _overwriteWeaponData;
    public bool OverwriteWeaponData => _overwriteWeaponData;

    [SerializeField] private EnemyWeaponUnit _newDataToUse;
    /// <summary>
    /// The new data to use with the weapon instead its default data.
    /// </summary>
    public EnemyWeaponUnit NewDataToUse => _newDataToUse;

    [SerializeField] private EnemyWeaponGameObjectData _usedWeapon;
    public EnemyWeaponGameObjectData UsedWeapon => _usedWeapon;
}

public class AttackWave : MonoBehaviour
{
    [SerializeField] private string _name;

    [SerializeField] private Enemy _enemy;
    public Enemy Enemy => _enemy;

    [SerializeField] private AttackWaveType _waveType;
    public AttackWaveType WaveType => _waveType;

    [Tooltip("Check this option if this attack wave should be used only when the enemy is in Critical Mode.")]
    [SerializeField] private bool _criticalAttackWave;
    public bool CriticalAttackWave => _criticalAttackWave;

    [SerializeField]
    [DrawItDisabled]
    private bool _waveStarted;
    public bool WaveStarted => _waveStarted;

    [TextArea]
    [SerializeField] private string _description;

    [SerializeField] private List<AttackWaveWeaponData> _weaponsUsed;
    public IReadOnlyList<AttackWaveWeaponData> WeaponsUsed => _weaponsUsed;

    private int _weaponUsedIndex;
    private float _timeCounter; 

    private void OnEnable()
    {
        this.GetComponentInAnyParent(out _enemy);
        _weaponUsedIndex = 0;
    }

    public void StartWave()
    {
        _waveStarted = true;

        print("StartAttackWave");
    }

    private void Update()
    {
        if (!WaveStarted)
            return;

        if (_weaponUsedIndex == WeaponsUsed.Count)
            _waveStarted = false;

        AttackWaveWeaponData weapon = WeaponsUsed[_weaponUsedIndex];

        int weaponIndex = Enemy.WeaponController.WeaponDataList.
            FindIndex(weaponData => weaponData.WeaponGameObjectData.gameObject.Equals(weapon.UsedWeapon.gameObject));
        EnemyWeaponDataListUnit weaponDataListUnit = Enemy.WeaponController.WeaponDataList[weaponIndex];

        int direction = Enemy.GetShotDirection(weapon.UsedWeapon);
        Transform spawn = weaponDataListUnit.WeaponGameObjectData.SpawnTransform;
        EnemyWeaponUnit scriptableObject = weapon.OverwriteWeaponData? weapon.NewDataToUse : weaponDataListUnit.WeaponScriptableObject;

        if (!scriptableObject.IsContinuous)
        {
            scriptableObject.Shoot(spawn, direction);
            _weaponUsedIndex++;
        }
        else
        {
            _timeCounter += Time.deltaTime;
            if(_timeCounter >= scriptableObject.ContinuousFireRate)
            {
                scriptableObject.Shoot(spawn, direction);
                _timeCounter = 0;
            }
        }

        if (weapon.OverwriteWeaponData)
            print("using overwritten data...");
    }
}