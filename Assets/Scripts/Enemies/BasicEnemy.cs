using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using UnityEngine.UIElements;

public class BasicEnemy : Enemy
{
    [Header("BasicEnemy Variables")]
    [SerializeField] protected float _movementDuration;
    [SerializeField] protected float _timeToInvertMovement;    
    [SerializeField] protected float _intervalBetweenAttacks;

    protected float _intervalBetweenAttacksTimeCounter;
    protected float _attackDurationTimeCounter;
    protected float _fireRateCounter;    

    private float _xDirection;
    private float _movementTimeCounter;
    private float _timeToInvertMovementTimeCounter;

    public override void Patrol()
    {
        Move();
    }

    protected override void Move()
    {
        if (_movementTimeCounter < _movementDuration)
        {
            _movementTimeCounter += Time.deltaTime;

            if (EnemyType == EnemyType.Land)
                MoveLandEnemy();
            else if (EnemyType == EnemyType.Flying)
                MoveFlyingEnemy();
        }
        else
        {
            _timeToInvertMovementTimeCounter += Time.deltaTime;

            if(_timeToInvertMovementTimeCounter > _timeToInvertMovement)
            {
                _movementTimeCounter = 0;
                _timeToInvertMovementTimeCounter = 0;
                Flip(transform.localScale.x < 0 ? 1 : -1);
                EnemyStateGeneralData.SetCurrentActionAsPerformed();
            }
        }
    }

    public override void ChasePlayer()
    {
        print("ChasePlayer");
        if(_detectedPlayer == null)
        {
            print("Lost player from sight. Returning to initial state...");
            ChangeState(EnemyStateGeneralData.InitialState);
            return;
        }

        if (ReachedAttackDistance())
        {
            print("Reached attack distance, so let's attack.");
            if(_collidedWithPlayer)
            {
                float distance = transform.position.x - _detectedPlayer.transform.position.x;
                Flip(distance < 0 ? 1 : -1);
            }
            ChangeState(EnemyState.Attacking);
        }
        else
        {
            if (EnemyType == EnemyType.Land)
                MoveLandEnemy();
            else if (EnemyType == EnemyType.Flying)
                MoveFlyingEnemy();
        }
    }

    protected virtual void MoveLandEnemy()
    {        
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _landCharacterProps.WallCheckCollider,
            _landCharacterProps.TerrainWithoutPlatformLayerMask,
            out Collider2D hitWall))
        {
            _xDirection = GetDirection() * _speed;

            Vector2 direction = new Vector2(_xDirection, Rigidbody.velocity.y);
            Rigidbody.velocity = direction;
            Flip((int)_xDirection);
        }
    }

    protected virtual void MoveFlyingEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(_flyingCharacterProps.FlyingCheckCollider,
            _flyingCharacterProps.FlyingLayerMask,
            out Collider2D hitWall))
        {
            _xDirection = GetDirection() * _speed;
            Vector2 direction = new Vector2(_xDirection, 0);
            transform.Translate(direction * Time.deltaTime);

            Flip((int)_xDirection);
        }
    }

    public override void Attack()
    {
        EnemyActionStatus status = EnemyStateGeneralData.CurrentState.Action.Status;
        print("Attack(); status: " + status + " Current state: " + EnemyStateGeneralData.CurrentState.EnemyState);
        
        if (status == EnemyActionStatus.Started)
        {
            if (_attackDurationTimeCounter <= 
                WeaponController.WeaponDataList[0].WeaponUnit.AttackDuration)
            {
                if(_fireRateCounter >
                    WeaponController.WeaponDataList[0].WeaponUnit.GetFireRate())
                {
                    Shoot(0);
                    _fireRateCounter = 0;
                }
                else
                    _fireRateCounter += Time.deltaTime;

                _attackDurationTimeCounter += Time.deltaTime;
            }
            else
            {
                //print("End of attack wave.");
                EnemyStateGeneralData.SetCurrentActionAsPerformed();            
            }
        }
        else if (status == EnemyActionStatus.Performed)
        {
            _intervalBetweenAttacksTimeCounter += Time.deltaTime;

            if (_intervalBetweenAttacksTimeCounter >= _intervalBetweenAttacks)
            {
                //print("Starting another attack wave...");
                _intervalBetweenAttacksTimeCounter = 0;
                _attackDurationTimeCounter = 0;
                _fireRateCounter = 0;
                EnemyStateGeneralData.CurrentState.Action.SetActionStatusStarted();
                return;
            }
        }

        if (!_attackCanceled && !ReachedAttackDistance())
        {
            print("Couldn't attack player anymore, so let's chase him.");
            _attackCanceled = true;
            ChangeState(EnemyState.ChasingPlayer);
        }
    }
}
