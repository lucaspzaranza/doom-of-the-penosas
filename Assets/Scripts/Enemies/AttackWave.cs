using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWave : MonoBehaviour
{
    [SerializeField] private string _name;

    [SerializeField] private AttackWaveType _waveType;
    public AttackWaveType WaveType => _waveType;

    [Tooltip("Check this option if this attack wave should be used only when the enemy is in Critical Mode.")]
    [SerializeField] private bool _criticalAttackWave;
    public bool CriticalAttackWave => _criticalAttackWave;

    [TextArea]
    [SerializeField] private string _description;

    //[Tooltip("Check this if this weapon has continuous fire, like a machinegun, for example.")]
    //[SerializeField] private bool _isContinuous;
    //public bool IsContinuous => _isContinuous;

    //[Tooltip("It'll be used only when the \"Is Continuous\" option be unchecked.")]
    //[DrawIfBoolEqualsTo("_isContinuous", comparedValue: false, elseDrawItDisabled: true)]
    //[SerializeField] private float _fireRate;
    //public float FireRate => _fireRate;

    //[Tooltip("It'll be used only when the \"Is Continuous\" option be marked.")]
    //[DrawIfBoolEqualsTo("_isContinuous", comparedValue: true, elseDrawItDisabled: true)]
    //[SerializeField] private float _continuousFireRate;
    //public float ContinuousFireRate => _continuousFireRate;

    //[SerializeField] private List<EnemyWeaponGameObjectData> _weaponsUsed;
    //public IReadOnlyList<EnemyWeaponGameObjectData> WeaponsUsed => _weaponsUsed;

    [SerializeField] private List<AttackWaveWeaponData> _weaponsUsed;
    public IReadOnlyList<AttackWaveWeaponData> WeaponsUsed => _weaponsUsed;
}

[System.Serializable]
public class AttackWaveWeaponData
{
    [Tooltip("Check this if you want to overwrite the current weapon data with new data. " +
    "It'll be useful if you want to use the same weapon to fire different types of projectiles, or " +
    "if you want to change the weapon fire pattern from a single to a continuous, for example.")]
    [SerializeField] private bool _overwriteWeaponData;
    public bool OverwriteWeaponData => _overwriteWeaponData;

    [SerializeField] private EnemyWeaponUnit _overwrittenData;
    public EnemyWeaponUnit OverwrittenData => _overwrittenData;

    [SerializeField] private EnemyWeaponGameObjectData _usedWeapon;
    public EnemyWeaponGameObjectData UsedWeapon => _usedWeapon;
}