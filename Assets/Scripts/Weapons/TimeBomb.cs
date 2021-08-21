using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TimeBomb : Grenade
{   
    public float remoteSpeed;
    public float overlapRadius;
    private bool isLeft = false;
    private bool fixedOnTheGround = false;
    [SerializeField] private Collider2D[] bombColliders = null;
    [SerializeField] private Transform cornerTransform = null;
    private Collider2D previousCollider;

    private PlayerInput playerInputActions;
    private InputAction timeBombExplosionAction;

    private void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInputActions.Player.TimeBombMovement.Enable();

        timeBombExplosionAction = playerInputActions.Player.Fire2Shot;
        timeBombExplosionAction.Enable();
    }

    private void OnDisable()
    {
        timeBombExplosionAction.Disable();
    }

    public override void Start()
    {
        base.Start();
        if(transform.localScale.x < 0) isLeft = true;
    }

    public override void Update()
    {
        CheckBombContact();
        if (fixedOnTheGround)
            RemoteMove();

        BombExplosionVerification();
    }

    private void BombExplosionVerification()
    {
        if(timeBombExplosionAction.triggered)
            Explode();
    }

    private void CheckBombContact()
    {
        for (int i = 0; i < bombColliders.Length ; i++)
        {
            Vector2 pos = bombColliders[i].transform.position;
            var other = Physics2D.OverlapCircle(pos, overlapRadius, interactableLayerMask);                   
            if(other == null) // Check if has collision with the layers from the Layer Mask (Enemy and Map)
            { //If other is null, the bomb has collided with nothing, and it will rotate 
              // to stick on the other map corner
                if(i == 0 && fixedOnTheGround) // Bottom collider          
                {                                                                                        
                    transform.position = cornerTransform.position;
                    transform.Rotate(Vector3.forward, isLeft? 90f : -90f);                                   
                    return;
                }                                 
            }   
            else
            {
                FixBombOnGround(other.gameObject);
                if(i == 1) // Frontal collider                                                   
                    transform.Rotate(Vector3.forward, isLeft? -90f : 90f); 
                previousCollider = other;
            }            
        }
    }

    private void RemoteMove()
    {
        float DPad = playerInputActions.Player.TimeBombMovement.ReadValue<float>();

        if (DPad > 0 && isLeft) Flip();
        else if (DPad < 0 && !isLeft) Flip();

        transform.Translate(Vector2.right * remoteSpeed * DPad * Time.deltaTime);
    }

    private void Flip()
    {
        isLeft = !isLeft;
        transform.localScale = new Vector2
            (transform.localScale.x * -1, transform.localScale.y);
    }

    private void FixBombOnGround(GameObject other)
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePosition;        
        fixedOnTheGround = true;
    }
}