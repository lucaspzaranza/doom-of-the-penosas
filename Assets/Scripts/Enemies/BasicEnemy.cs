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
    [SerializeField] protected bool _rightDirection;

    private float _xDirection;
    private float _movementTimeCounter;
    private float _timeToInvertMovementTimeCounter;

    public override void Patrol()
    {
        Move();
    }

    protected override void Move()
    {
        if (_rightDirection && transform.localScale.x < 0)
            Flip();
        else if (!_rightDirection && transform.localScale.x > 0)
            Flip();

        if(_movementTimeCounter < _movementDuration)
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
                _rightDirection = !_rightDirection;
            }
        }
    }

    protected virtual void MoveLandEnemy()
    {        
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _landCharacterProps.WallCheckCollider,
            _landCharacterProps.TerrainWithoutPlatformLayerMask,
            out Collider2D hitWall))
        {
            //bool chooseRight = Random.Range(0, 2) == 1;
            _xDirection = GetDirection() * _speed;
            Vector2 direction = new Vector2(_xDirection, Rigidbody.velocity.y);
            Rigidbody.velocity = direction;
        }
    }

    protected virtual void MoveFlyingEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(_flyingCharacterProps.FlyingCheckCollider,
            _flyingCharacterProps.FlyingLayerMask,
            out Collider2D hitWall))
        {
            //print("I'm flyin'...");
            //_xDirection = chooseRight ? _speed * 1 : _speed * -1;
            _xDirection = GetDirection() * _speed;
            Vector2 direction = new Vector2(_xDirection, 0);
            transform.Translate(direction * Time.deltaTime);
        }
    }    
}
