using UnityEngine;

public class Grenade : Projectile
{
    protected Rigidbody2D rb2D;
    private float YDecreaseOffset = 2f;

    protected bool IsOnAir
    {
        get { return rb2D.velocity.y != 0; }
    }

    public override void Start()
    {
        base.Start();        
        ThrowGrenade(speed, Mathf.Abs(speed) - YDecreaseOffset);
    }

    public override void Update()
    {
        if(TouchedProjectileInteractable) Explode();
    }

    public virtual void ThrowGrenade(float x, float y)
    {
        rb2D = GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(x, y);
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }

    public virtual void Explode()
    {
        Destroy(this.gameObject);
    }
}