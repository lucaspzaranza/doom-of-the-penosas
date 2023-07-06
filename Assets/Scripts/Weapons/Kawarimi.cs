using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Kawarimi : Grenade
{
    public float torque;
    [HideInInspector] public GameObject penosa;
    private Collider2D coll2D;

    private PlayerInput playerInputActions;
    private InputAction kawarimiExplosionAction;

    private void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInputActions.Player.TimeBombMovement.Enable();

        kawarimiExplosionAction = playerInputActions.Player.Fire2;
        kawarimiExplosionAction.Enable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        kawarimiExplosionAction.Disable();
    }

    public void Start()
    {      
        ThrowGrenade(Speed, Mathf.Abs(Speed));
        rb2D.AddTorque(torque, ForceMode2D.Impulse);
        coll2D = GetComponent<Collider2D>();
    }

    public override void Update()
    {
        if(kawarimiExplosionAction.triggered) KawarimiNoJutsu();
        if(rb2D.velocity == Vector2.zero)
        {
            if(!coll2D.isTrigger) coll2D.isTrigger = true;
            rb2D.bodyType = RigidbodyType2D.Static;
        }
    }

    private void DestroyKawarimi()
    {
        Destroy(gameObject);
    }

    private void KawarimiNoJutsu()
    {
        if (penosa != null)
        {
            rb2D.bodyType = RigidbodyType2D.Dynamic;
            coll2D.isTrigger = false;
            Vector3 penosaPosition = penosa.transform.position;
            penosa.transform.position = transform.position;
            transform.position = new Vector2(penosaPosition.x, penosaPosition.y);
            kawarimiExplosionAction.Disable();
        }

        Invoke(nameof(DestroyKawarimi), 2f);
    }
}