using UnityEngine;

public class TimeBomb : Grenade
{   
    public float remoteSpeed;
    public float overlapRadius;
    private bool isLeft = false;
    private bool sticked = false;
    [SerializeField] private Collider2D[] bombColliders = null;
    [SerializeField] private Transform cornerTransform = null;
    private Collider2D previousCollider;

    public override void Start()
    {
        base.Start();
        if(transform.localScale.x < 0) isLeft = true;
    }

    public override void Update()
    {
        CheckBombContact();
        if(sticked)
            RemoteMove(); 

        if(Input.GetButtonDown("Fire2")) Explode();
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
                if(i == 0 && sticked) // Bottom collider          
                {                                                                                        
                    transform.position = cornerTransform.position;
                    transform.Rotate(Vector3.forward, isLeft? 90f : -90f);                                   
                    return;
                }                                 
            }   
            else
            {
                StickBomb(other.gameObject);
                if(i == 1) // Frontal collider                                                   
                    transform.Rotate(Vector3.forward, isLeft? -90f : 90f); 
                previousCollider = other;
            }            
        }
    }

    private void RemoteMove()
    {       
        float DPad = Input.GetAxisRaw("D Pad");
        if(DPad > 0 && isLeft) Flip();
        else if (DPad < 0 && !isLeft) Flip();        
 
        transform.Translate(Vector2.right * remoteSpeed * DPad * Time.deltaTime);
    }    

    private void Flip()
    {
        isLeft = !isLeft;
        transform.localScale = new Vector2
            (transform.localScale.x * -1, transform.localScale.y);
    }

    private void StickBomb(GameObject other)
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePosition;        
        sticked = true;
    }
}