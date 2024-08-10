using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using UnityEngine;

/// <summary>
/// Use this class to manage different kind of attacks performed by the enemy. Mostly used by bosses enemies.
/// </summary>
[System.Serializable]
public class AttackWavesController
{    
    private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] private AttackWave _currentWave;
    public AttackWave CurrentWave => _currentWave;

    [SerializeField] private List<AttackWave> _attackWaves;
    public IReadOnlyList<AttackWave> AttackWaves => _attackWaves;

    public void SetWeaponController(EnemyWeaponController controller)
    {
        _weaponController = controller;
    }

    public void ChooseRandomAttackWave(bool isCritical)
    {
        IReadOnlyList<AttackWave> selectedWaves = AttackWaves.Where(atkWave => atkWave.CriticalAttackWave == isCritical).ToList();
        int randomIndex = Random.Range(0, selectedWaves.Count);
        _currentWave = selectedWaves[randomIndex];
    }

    public void ChooseAttackWave(int waveIndex)
    {
        _currentWave = AttackWaves[waveIndex];
    }

    public void StartAttackWave()
    {
        if(!CurrentWave.WaveStarted)
            CurrentWave.StartWave();
    }
}
