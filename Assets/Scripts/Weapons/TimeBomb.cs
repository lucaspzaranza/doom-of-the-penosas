using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TimeBomb : Grenade
{   
    public float remoteSpeed;
    public float overlapRadius;
    [SerializeField] private Collider2D[] bombColliders = null;
    [SerializeField] private Transform cornerTransform = null;

    private bool fixedOnTheGround = false;
    private Collider2D previousCollider;
    private bool isLeft = false;
    private PlayerInput playerInputActions;
    private InputAction timeBombExplosionAction;

    private void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInputActions.Player.TimeBombMovement.Enable();

        timeBombExplosionAction = playerInputActions.Player.Fire2;
        timeBombExplosionAction.Enable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        timeBombExplosionAction.Disable();
    }

    private void Start()
    {
        isLeft = transform.localScale.x < 0;
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

    // Sobrescrevendo o método de explosão porque as bombas nível 2 não estão utilizando pooling
    public override void Explode()
    {
        if (SharedFunctions.HitSomething(_collider, interactableLayerMask, out Collider2D hitObject))
        {
            TryToDamageEnemy(ref hitObject);
        }

        Destroy(gameObject);
    }

    private void CheckBombContact()
    {
        for (int i = 0; i < bombColliders.Length ; i++)
        {
            Vector2 pos = bombColliders[i].transform.position;

            // Checa se tem colisão com os layers Enemy ou Map. Se for nulo, a bomba não colidiu com nada,
            // e sofrerá uma rotação pra se adaptar ao terreno.
            var other = Physics2D.OverlapCircle(pos, overlapRadius, interactableLayerMask);                   
            if(other == null)
            {
                if(i == 0 && fixedOnTheGround) // Bottom collider          
                {                         
                    // Parou de ter colisão no chão, com o Bottom Collider não tendo encontrado nenhum Collider por ter retornado nulo.
                    transform.position = cornerTransform.position;
                    transform.Rotate(Vector3.forward, isLeft? 90f : -90f);                                   
                    return;
                }                                 
            }   
            else
            {
                FixBombOnGround(other.gameObject);
                // Teve colisão com o Frontal Collider, e sofrerá a rotação necessária.
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