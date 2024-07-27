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
    private float _yDirection;
    private float _movementTimeCounter;
    private float _timeToInvertMovementTimeCounter;

    public override void Patrol()
    {
        if(CanMove)
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
            _isMoving = false;

            if(_timeToInvertMovementTimeCounter > _timeToInvertMovement)
            {
                _movementTimeCounter = 0;
                _timeToInvertMovementTimeCounter = 0;

                if(EnemyType == EnemyType.Flying)
                {
                    if(FlyingChaseMode == FlyingChaseMode.Diagonal)
                    {
                        FlipVerticalDirection();
                        Flip(transform.localScale.x < 0 ? 1 : -1);
                    }
                    else if (FlyingChaseMode == FlyingChaseMode.Vertical)
                        FlipVerticalDirection();
                    else if(FlyingChaseMode == FlyingChaseMode.Mixed)
                    {
                        SetRandomFlyingMixedMovement();

                        if (_moveFlyingOnHorizontal)
                            Flip(transform.localScale.x < 0 ? 1 : -1);
                        
                        if(_moveFlyingOnVertical)
                            FlipVerticalDirection();
                    }
                }
                else // Land Character or Horizontal Chase Mode
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
            _isMoving = false;
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
            _isMoving = false;
            ChangeState(EnemyState.Attacking);
        }
        else if(CanMove)
        {
            if (EnemyType == EnemyType.Land)
                MoveLandEnemy();
            else if (EnemyType == EnemyType.Flying)
                MoveFlyingEnemy();
        }
    }

    public override void TakeDamage(int damage, bool force = false)
    {
        base.TakeDamage(damage, force);

        if (_tookDamage && ChangeStateAfterDamage && State == EnemyState.Idle)
        {
            //print($"Took damage, changing state to {StateAfterDamage}...");
            ChangeState(StateAfterDamage);
        }
    }

    protected virtual void MoveLandEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _landCharacterProps.WallCheckCollider,
            _landCharacterProps.TerrainWithoutPlatformLayerMask,
            out Collider2D hitWall)
        )
        {
            _isMoving = true;
            _xDirection = GetDirection() * _speed;

            Vector2 direction = new Vector2(_xDirection, Rigidbody.velocity.y);
            Rigidbody.velocity = direction;
            Flip((int)_xDirection);
        }
        else
            _isMoving = false;
    }

    protected virtual void MoveFlyingEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _flyingCharacterProps.FlyingCheckCollider,
            _flyingCharacterProps.FlyingLayerMask,
            out Collider2D hitWall))
        {
            _isMoving = true;
            _xDirection = 0f;
            _yDirection = 0f;

            if(FlyingChaseMode == FlyingChaseMode.Horizontal)
                _xDirection = GetDirection() * _speed;
            else if(FlyingChaseMode == FlyingChaseMode.Vertical)
                _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
            else if (FlyingChaseMode == FlyingChaseMode.Diagonal)
            {
                _xDirection = GetDirection() * _speed;
                _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
            }
            else if (FlyingChaseMode == FlyingChaseMode.Mixed)
            {
                _xDirection = _moveFlyingOnHorizontal ? GetDirection() * _speed : 0f;
                _yDirection = _moveFlyingOnVertical? GetDirection(calculateInVerticalAxis: true) * _speed : 0f;
            }

            Vector2 direction = new Vector2(_xDirection, _yDirection);
            transform.Translate(direction * Time.deltaTime);

            Flip((int)_xDirection);
        }
        else
            _isMoving = false;
    }

    public override void Attack()
    {
        EnemyActionStatus status = EnemyStateGeneralData.CurrentState.Action.Status;
        //print("Attack(); status: " + status + " Current state: " + EnemyStateGeneralData.CurrentState.EnemyState);
        
        if (status == EnemyActionStatus.Started)
        {
            if (_attackDurationTimeCounter <= 
                WeaponController.WeaponDataList[0].WeaponScriptableObject.AttackDuration)
            {
                if(_fireRateCounter >
                    WeaponController.WeaponDataList[0].WeaponScriptableObject.GetFireRate())
                {
                    //SelectWeaponAndShoot();
                    print("SHOOT");
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
                return;
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

        if (!_attackCanceled && LostPlayerFromSight())
        {
            _attackCanceled = true;
            _fireRateCounter = 0;
            _attackDurationTimeCounter = 0;

            if (InstantAttack)
            {
                print("Couldn't attack player anymore, so let's return to Initial State.");
                ChangeState(EnemyStateGeneralData.InitialState);
            }
            else
            {
                print("Couldn't attack player anymore, so let's chase him.");
                ChangeState(EnemyState.ChasingPlayer);
            }
        }
    }

    private bool LostPlayerFromSight()
    {
        if (!IsUsingWeaponWhichRotates)
        {
            // If it's NOT an InstantAttack, it means the enemy must approach the player to start the attack.
            // Therefore it must detect if the player is out of a strikeable distance to set him lost from sight.
            if (!InstantAttack) 
                return !ReachedAttackDistance();
            // Else, it means the enemy mustn't calculate any distance from player to start its attack.
            // As soon the enemy detects players presence, it'll start attacking.
            // So the detection will be on player's presence without any need of attack calculation distance.
            else
                return !DetectedPlayer();
        }
        else
        {
            // If the weapon rotates towards player and if the enemy doesn't detect any player's presence, 
            // it means the enemy lost him from sight;
            if (!DetectedPlayer())
                return true;

            // Else and there's some player detected, the enemy must check if the player is out of some
            // strikeable distance to lose him from sight.
            return !ReachedAttackDistance();
        }
    }
}
