using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chickencopter : RideArmor
{
    [SerializeField] private Animator _propellerAnimator;
    [SerializeField] private Collider2D _verticalCheckCollider = null;
    [SerializeField] private Transform _activatorTransform = null;
    [SerializeField] private float _upperCheckY;
    [SerializeField] private float _lowerCheckY;

    private bool _isGrounded = true;
    public bool IsGrounded => _isGrounded;

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(_activatorTransform.position, 
            PlayerConsts.ChickencopterOverlapCircleDiameter, _terrainLayerMask);
        print(_isGrounded);
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
        if (_isGrounded)
        {
            print("Para normalmente");

            _propellerAnimator.enabled = false;
            _propellerAnimator.transform.rotation = Quaternion.identity;
            Player.Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
            RigiBody2DComponent.gravityScale = PlayerConsts.DefaultGravity;
            base.Eject();
        }
        else
            print("Menino destrói essa bagaça!");
    }
}
