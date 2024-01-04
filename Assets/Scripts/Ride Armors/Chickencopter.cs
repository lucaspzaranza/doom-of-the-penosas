using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chickencopter : RideArmor
{
    [SerializeField] private Animator _propellerAnimator;
    [SerializeField] protected LayerMask _abandonedHitLayerMask;
    [SerializeField] protected Vector2 _overlapBoxSize;
    [SerializeField] private Collider2D _verticalCheckCollider = null;
    [SerializeField] private Collider2D _abandonedCheckCollider = null;
    [SerializeField] private Transform _activatorTransform = null;
    [SerializeField] protected float _groundDistance;
    [SerializeField] private float _upperCheckY;
    [SerializeField] private float _lowerCheckY;

    private bool _chickencopterAbandoned = false;

    private bool _isNearGround = true;
    public bool IsGrounded => _isNearGround;

    private void Update()
    {
        _isNearGround = Physics2D.OverlapBox(_activatorTransform.position, 
            _overlapBoxSize, 0f, _terrainLayerMask);
        print(_isNearGround);

        if(_chickencopterAbandoned)
        {
            bool hitDestroyerStuff = Physics2D.OverlapBox(_abandonedCheckCollider.transform.position,
                new Vector2(0f, _groundDistance), 0f, _abandonedHitLayerMask);

            if(hitDestroyerStuff)
            {
                // Add explosion effect here...
                print("CABUM!");
                Destroy(gameObject);
            }
        }
    }

    public override void Equip(Penosa player, PlayerController playerController)
    {
        base.Equip(player, playerController);
        player.Rigidbody2D.gravityScale = 0f;
        RigiBody2DComponent.gravityScale = 0f;
        _propellerAnimator.enabled = true;
    }

    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        if (SharedFunctions.HitWall(_wallCheckCollider, _terrainLayerMask, out Collider2D hitWall) && 
            (direction.x != 0 && hitWall.transform.position.x != transform.position.x))
            return;

        float posX = _verticalCheckCollider.transform.localPosition.x;
        if (direction.y > 0)
            _verticalCheckCollider.transform.localPosition = new Vector2(posX, _upperCheckY);
        else if (direction.y < 0)
            _verticalCheckCollider.transform.localPosition = new Vector2(posX, _lowerCheckY);

        if (SharedFunctions.HitWall(_verticalCheckCollider, _terrainLayerMask, out Collider2D upperHitWall) &&
            (direction.y != 0 && upperHitWall.transform.position.y != transform.position.y))
            return;

        transform.Translate(direction * Time.deltaTime);
    }

    public override void Eject()
    {
        Player.Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        RigiBody2DComponent.gravityScale = PlayerConsts.DefaultGravity;

        if (_isNearGround)
        {
            _propellerAnimator.enabled = false;
            _propellerAnimator.transform.rotation = Quaternion.identity;
            base.Eject();
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
            _abandonedCheckCollider.gameObject.SetActive(true);
            _chickencopterAbandoned = true;
        }
    }
}
