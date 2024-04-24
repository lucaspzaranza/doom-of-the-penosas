using System;
using UnityEngine;

public class Grenade : Projectile
{
    protected Rigidbody2D rb2D;
    protected float YDecreaseOffset = 2f;

    protected bool IsOnAir
    {
        get { return rb2D.velocity.y != 0; }
    }

    public override void Update()
    {
        if(SharedFunctions.HitSomething(_collider, InteractableLayerMask, out Collider2D hitObject))
        {
            DamageEnemy(ref hitObject);
            Explode();
        }
    }

    protected void ThrowGrenade(float x, float y)
    {
        rb2D = GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(x, y);
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }

    public virtual void Explode()
    {
        //ObjectPool.instance.ReturnGameObject(gameObject);

        // Adicionar aqui lógica de animação de explosão...
        OnReturnProjectileToPool?.Invoke(gameObject);
    }

    public void CallThrowGrenade()
    {
        ThrowGrenade(Speed, Mathf.Abs(Speed) - YDecreaseOffset);
    }

    public virtual void OnDisable()
    {
        Speed = Mathf.Abs(Speed); // Resetando a velocidade pro seu valor positivo.
    }
}