using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chickencopter : RideArmor
{
    [SerializeField] private LayerMask _abandonedHitLayerMask;
    [SerializeField] private Animator _propellerAnimator;
    [SerializeField] private SpriteRenderer _propellerSpriteRenderer;
    [SerializeField] private Collider2D _verticalCheckCollider = null;
    [SerializeField] private Collider2D _abandonedCheckCollider = null;
    [SerializeField] private Transform _activatorTransform = null;
    [SerializeField] private float _distanceToGround;
    [SerializeField] private float _upperCheckY;
    [SerializeField] private float _lowerCheckY;
    [SerializeField] private float _fallRate;
    [SerializeField] private int _fallDamage;

    private Vector2 _currentDirection;

    private bool _isNearGround = true;
    public bool IsGrounded => _isNearGround;

    private bool _chickencopterAbandoned = false;
    public bool ChickencopterAbandoned => _chickencopterAbandoned;

    protected void Update()
    {
        _isNearGround = Physics2D.Raycast(_activatorTransform.position, 
            Vector2.down, _distanceToGround, _terrainLayerMask);

        if(_chickencopterAbandoned)
        {
            if(SharedFunctions.HitSomething(_abandonedCheckCollider, 
                _abandonedHitLayerMask, out Collider2D hitObject))
            {
                if (hitObject.TryGetComponent(out Enemy enemy))
                    enemy.TakeDamage(_fallDamage);

                // Add explosion effect here...
                Destroy(gameObject);
            }
        }
    }

    public override void Equip(Penosa player, PlayerController playerController)
    {
        base.Equip(player, playerController);
        RigiBody2DComponent.gravityScale = 0f;
        player.Rigidbody2D.velocity = Vector2.zero;
        _propellerAnimator.enabled = true;
    }

    public override void Move(Vector2 direction)
    {        
        if (SharedFunctions.HitSomething(_wallCheckCollider, _terrainWithoutPlatformLayerMask, out Collider2D hitWall) && 
            (direction.x != 0 && hitWall.transform.position.x != transform.position.x))
            return;

        float posX = _verticalCheckCollider.transform.localPosition.x;
        if (direction.y > 0)
            _verticalCheckCollider.transform.localPosition = new Vector2(posX, _upperCheckY);
        else if (direction.y < 0)
            _verticalCheckCollider.transform.localPosition = new Vector2(posX, _lowerCheckY);

        if (SharedFunctions.HitSomething(_verticalCheckCollider, _terrainLayerMask, out Collider2D upperHitWall) &&
            (direction.y != 0 && upperHitWall.transform.position.y != transform.position.y))
            return;

        transform.Translate(direction * Time.deltaTime);
        _currentDirection = direction;
        base.Move(direction);
    }

    public override void Eject()
    {
        Player.Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        RigiBody2DComponent.gravityScale = PlayerConsts.DefaultGravity;
        SetEquippedPlayerActivation(Player, false);

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
            RigiBody2DComponent.velocity = _currentDirection * _fallRate;
        }
    }

    public override void Blink(Color newColor)
    {
        base.Blink(newColor);
        _propellerSpriteRenderer.color = newColor;
    }
}
