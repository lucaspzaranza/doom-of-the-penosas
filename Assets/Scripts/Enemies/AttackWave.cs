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

public struct WaveData
{
    public AttackWaveWeaponData Weapon { get; set; }
    public int Direction { get; set; }
    public Transform Spawn { get; set; }
    public EnemyWeaponUnit ScriptableObject { get; set; }
}

public class AttackWave : MonoBehaviour
{
    [SerializeField] private string _name;

    [TextArea]
    [SerializeField] private string _description;

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

    [SerializeField] private List<AttackWaveWeaponData> _weaponsUsed;
    public IReadOnlyList<AttackWaveWeaponData> WeaponsUsed => _weaponsUsed;

    [DrawIfEnumEqualsTo("_waveType", new AttackWaveType(), AttackWaveType.Sequence)]
    [Tooltip("The needed time to use the next weapon at a sequence wave.")]
    [SerializeField] private float _changeWeaponInterval;
    public float ChangeWeaponInterval => _changeWeaponInterval;

    private int _weaponUsedIndex;
    private int _prevWeaponUsedIndex;
    private float _atkDurationtimeCounter; 
    private float _fireRateAtkCounter;
    private float _timeToChangeWeapon;
    private WaveData _waveData;

    private void OnEnable()
    {
        this.GetComponentInAnyParent(out _enemy);
        _weaponUsedIndex = 0;
    }

    public void StartWave()
    {
        _waveData = GetWaveData(_weaponUsedIndex);
        _fireRateAtkCounter = _waveData.ScriptableObject.FireRate;
        _prevWeaponUsedIndex = _weaponUsedIndex;
        _waveStarted = true;
        print("StartAttackWave");
    }

    private void Update()
    {
        if (!WaveStarted || _waveData.ScriptableObject == null)
            return;

        if (WaveType == AttackWaveType.Sequence)
            SequenceAttackWave();
        else
            CombinedAttackWave();
    }

    private WaveData GetWaveData(int index)
    {
        AttackWaveWeaponData weapon = WeaponsUsed[index];

        int weaponIndex = Enemy.WeaponController.WeaponDataList.
            FindIndex(weaponData => weaponData.WeaponGameObjectData.gameObject.Equals(weapon.UsedWeapon.gameObject));
        EnemyWeaponDataListUnit weaponDataListUnit = Enemy.WeaponController.WeaponDataList[weaponIndex];

        int direction = Enemy.GetShotDirection(weapon.UsedWeapon);
        Transform spawn = weaponDataListUnit.WeaponGameObjectData.SpawnTransform;
        EnemyWeaponUnit scriptableObject = weapon.OverwriteWeaponData ? weapon.NewDataToUse : weaponDataListUnit.WeaponScriptableObject;

        return new WaveData()
        {
            Weapon = weapon,
            Direction = direction,
            Spawn = spawn,
            ScriptableObject = scriptableObject
        };
    }

    private void SequenceAttackWave()
    {
        if (_weaponUsedIndex == WeaponsUsed.Count)
        {
            print("All attack waves were successfully performed.");
            _waveStarted = false;
            _weaponUsedIndex = 0;
            return;
        }

        if(_prevWeaponUsedIndex != _weaponUsedIndex)
        {
            _waveData = GetWaveData(_weaponUsedIndex);
            _fireRateAtkCounter = _waveData.ScriptableObject.FireRate;
        }
       
        _atkDurationtimeCounter += Time.deltaTime;
        if (_atkDurationtimeCounter >= _waveData.ScriptableObject.AttackDuration)
        {
            _timeToChangeWeapon += Time.deltaTime;
            if (_timeToChangeWeapon >= ChangeWeaponInterval)
            {
                _atkDurationtimeCounter = 0f;
                _timeToChangeWeapon = 0f;
                _weaponUsedIndex++;
                print("End of attack wave, going to the next one.");
                return;
            }
        }
        else
        {
            _fireRateAtkCounter += Time.deltaTime;
            if (_fireRateAtkCounter >= _waveData.ScriptableObject.FireRate)
            {
                _waveData.ScriptableObject.Shoot(_waveData.Spawn, _waveData.Direction);
                _fireRateAtkCounter = 0;
            }
        }

        if (_waveData.Weapon.OverwriteWeaponData)
            print("using overwritten data...");

        _prevWeaponUsedIndex = _weaponUsedIndex;
    }

    private void CombinedAttackWave()
    {

    }
}