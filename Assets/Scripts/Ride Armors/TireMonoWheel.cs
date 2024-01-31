using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TireMonoWheel : RideArmor
{
    [Header("Specific Props")]
    [SerializeField] private GameObject _wheel;
    [SerializeField] private GameObject _equippedPlayerContainer;
    [SerializeField] private GameObject _tireAndWheelContainer;
    [SerializeField] private GameObject _tireAndWheelChangedPivotContainer;
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] protected SpriteRenderer _innerPartSR;
    [SerializeField] protected SpriteRenderer _innerPartLowerPivotSR;
    [SerializeField] protected SpriteRenderer _tireLowerPivotSR;
    [SerializeField] private Transform _groundCheck = null;
    [SerializeField] private float _wheelSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private PhysicsMaterial2D _bouncinessMaterial;
    [SerializeField] private SpringAnimation _springAnimation;

    private bool _isGrounded;
    private bool _jumped;
    private float _initYScale;
    private Color _innerPartInitColor;

    public override void OnEnable()
    {
        base.OnEnable();
        _collider.sharedMaterial = _bouncinessMaterial;
        _initYScale = _tireLowerPivotSR.transform.localScale.y;
    }

    private void Start()
    {
        _innerPartInitColor = _innerPartSR.color;
    }

    protected override void Update()
    {
        base.Update();

        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position,
            PlayerConsts.OverlapCircleDiameter, _terrainLayerMask);

        if(_isGrounded && _rb.velocity.y < 0 && 
            !_tireAndWheelContainer.activeSelf && _collider.sharedMaterial == null)
        {
            ChangeTireAndWheelContainers(true);
            ChangeTireAndWheelContainers(false);
            _jumped = false;
        }

        if(Player != null)
            Player.transform.position = gameObject.transform.position;
    }

    public override void Equip(Penosa player, PlayerController playerController)
    {
        base.Equip(player, playerController);

        _collider.sharedMaterial = null;
    }

    public override void Move(Vector2 direction)
    {
        if (SharedFunctions.HitWall(_wallCheckCollider, _terrainWithoutPlatformLayerMask, out Collider2D hitWall))
            return;

        RigiBody2DComponent.velocity = new Vector2(direction.x, RigiBody2DComponent.velocity.y);
        if (direction.x != 0)
        {
            if (_tireAndWheelChangedPivotContainer.activeSelf && !_jumped && !_springAnimation.Activated)
                ChangeTireAndWheelContainers(true);

            _wheel.transform.Rotate(Vector3.forward, _wheelSpeed * -direction.x * Time.deltaTime);
            base.Move(direction);
        }
    }

    public override void Eject()
    {
        SetEquippedPlayerActivation(Player, false);

        base.Eject();
    }

    public override void Jump()
    {
        base.Jump();

        if(_collider.sharedMaterial == null)
            _collider.sharedMaterial = _bouncinessMaterial;

        if (_isGrounded)
        {            
            if(_tireAndWheelChangedPivotContainer.activeSelf)
                ChangeTireAndWheelContainers(true);

            _jumped = true;
            ChangeTireAndWheelContainers(false);
            Invoke(nameof(AddForce), 0.1f);
        }
    }

    private void AddForce()
    {
        RigiBody2DComponent.AddForce(Vector2.up * _jumpForce);
    }

    private void ChangeTireAndWheelContainers(bool activateNormalTireAndWhel)
    {
        _tireAndWheelContainer.SetActive(activateNormalTireAndWhel);
        _tireAndWheelChangedPivotContainer.SetActive(!activateNormalTireAndWhel);

        if (_tireAndWheelChangedPivotContainer.activeSelf)
            _equippedPlayer.transform.SetParent(_innerPartLowerPivotSR.transform);
        else
            _equippedPlayer.transform.SetParent(_innerPartSR.transform);
    }


    public void DeactivateBounciness()
    {
        _collider.sharedMaterial = null;
    }

    public override void Blink(Color newColor)
    {
        base.Blink(newColor);

        if (newColor.a > 0)
        {
            _innerPartSR.color = _innerPartInitColor;
            _innerPartLowerPivotSR.color = _innerPartInitColor;

        }
        else
        {
            _innerPartSR.color = newColor;
            _innerPartLowerPivotSR.color = newColor;
        }

        _tireLowerPivotSR.color = newColor;
    }
}
