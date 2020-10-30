using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public LayerMask interactableLayerMask;

    protected bool TouchedProjectileInteractable
    {
        get { return Physics2D.OverlapCircle(transform.position, 0.1f, interactableLayerMask);}
    }

    public virtual void Start()
    {
        if(speed < 0)
            transform.localScale = new Vector2
            (-transform.localScale.x, transform.localScale.y);
    }

    public virtual void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if(TouchedProjectileInteractable) Destroy(this.gameObject);
    }
}