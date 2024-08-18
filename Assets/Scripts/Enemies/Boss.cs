using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Tooltip("The life value necessary to activate the Boss Critical Mode.")]
    [SerializeField] protected int _criticalModeLife;
    public int CriticalModeLife => _criticalModeLife;

    protected override void OnEnable()
    {
        // Bosses always attack in attack waves.
        _hasAttackWaves = true;
        base.OnEnable();

        EndOfAttackWavesEventSetup();
    }

    protected override void Update()
    {
        // Temporary for test usage only
        if (Input.GetKeyDown(KeyCode.O))
        {
            if(State != EnemyState.Attacking)
                ChangeState(EnemyState.Attacking);

            SelectWeaponOrWaveAndAttack();
        }

        if (_weaponsWhichRotateTowardsPlayer.Count > 0)
            RotateWeaponsTowardsPlayer();

        if (CollidedWithPlayer(out _detectedPlayer))
        {
            int direction = GetDirection();
            //print($"Let's flip the enemy direction. Current direction: {direction}");
            Flip(direction);
        }

        if (EnemyStateGeneralData.CurrentState != null)
        {
            PerformActionBasedOnState(State);

            CheckForNewStateTimeCounter += Time.deltaTime;
            if (CheckForNewStateTimeCounter > TimeToCheckNewState)
            {
                CheckForNewStateTimeCounter = 0f;
                CheckForNewRandomState(State);
            }
        }
        else
            CheckForNewRandomState(State);

        if(!IsCritical && Life <= CriticalModeLife)
            SetCriticalMode();
    }

    protected virtual void SetCriticalMode()
    {
        print("Activating Critical Mode...");
        _isCritical = true;
        AttackWavesController.ChooseRandomAttackWave(IsCritical);
    }

    protected virtual void EndOfAttackWavesEventSetup()
    {
        foreach (var atkWave in AttackWavesController.AttackWaves)
        {
            atkWave.OnAttackWaveEnd += HandleOnAttackWaveEnd;
        }
    }

    protected virtual void EndOfAttackWavesEventDispose()
    {
        foreach (var atkWave in AttackWavesController.AttackWaves)
        {
            atkWave.OnAttackWaveEnd -= HandleOnAttackWaveEnd;
        }
    }

    private void HandleOnAttackWaveEnd(AttackWave attackWave)
    {
        EnemyStateGeneralData.SetCurrentActionAsPerformed();

        //print("Time elapsed: " + attackWave.TotalTimeElapsed + " time for new state: " + TimeToCheckNewState);

        if (attackWave.TotalTimeElapsed < TimeToCheckNewState)
            CheckForNewStateTimeCounter = TimeToCheckNewState;
        else
            CheckForNewStateTimeCounter = 0f;
    }

    public override void Attack()
    {
        EnemyActionStatus status = EnemyStateGeneralData.CurrentState.Action.Status;
        //print("Attack status: " + status + " Current state: " + EnemyStateGeneralData.CurrentState.EnemyState);

        if (status == EnemyActionStatus.Started)
            SelectWeaponOrWaveAndAttack();
        //else if (status == EnemyActionStatus.Performed)
        //{
        //    _intervalBetweenAttacksTimeCounter += Time.deltaTime;

        //    if (_intervalBetweenAttacksTimeCounter >= _intervalBetweenAttacks)
        //    {
        //        _intervalBetweenAttacksTimeCounter = 0;
        //        _attackDurationTimeCounter = 0;
        //        _fireRateCounter = 0;
        //        EnemyStateGeneralData.CurrentState.Action.SetActionStatusStarted();
        //        return;
        //    }
        //}
    }
}
