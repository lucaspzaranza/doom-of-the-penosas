using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using UnityEngine.UIElements;

public class BasicEnemy : Enemy
{
    public bool chooseRight;
    protected bool _isLeft;
    protected float _xDirection;

    protected void Update()
    {
        Patrol();
    }

    public override void Patrol()
    {
        //Move();
    }

    protected override void Move()
    {
        if (IsLandCharacter)
            MoveLandEnemy();
        else
            MoveFlyingEnemy();
    }

    protected virtual void MoveLandEnemy()
    {
        if (chooseRight && transform.localScale.x < 0)
            Flip();
        else if (!chooseRight && transform.localScale.x > 0)
            Flip();

        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _landCharacterProps.WallCheckCollider,
            _landCharacterProps.TerrainWithoutPlatformLayerMask,
            out Collider2D hitWall)
        )
        {
            //bool chooseRight = Random.Range(0, 2) == 1;
            _xDirection = chooseRight ? _speed * 1 : _speed * -1;
            Vector2 direction = new Vector2(_xDirection, Rigidbody.velocity.y);
            Rigidbody.velocity = direction;
        }
    }

    protected virtual void MoveFlyingEnemy()
    {

    }

    protected virtual void Flip()
    {
        _isLeft = !_isLeft;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    protected override void CheckForNewState()
    {

    }
}
