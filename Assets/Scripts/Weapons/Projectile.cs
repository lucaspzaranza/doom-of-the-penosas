using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    public int damage;
    public bool isMissile = false;
    public LayerMask interactableLayerMask;

    public float Speed 
    {
        get => _speed;

        set => _speed = value;
    }

    protected bool TouchedProjectileInteractable
    {
        get { return Physics2D.OverlapCircle(transform.position, 0.1f, interactableLayerMask);}
    }

    public virtual void Start()
    {
        if(Speed < 0)
            transform.localScale = new Vector2
            (-transform.localScale.x, transform.localScale.y);
    }

    public virtual void Update()
    {
        transform.Translate(Vector3.right * Speed * Time.deltaTime);
        if (TouchedProjectileInteractable && !isMissile)
            ObjectPool.instance.ReturnGameObject(gameObject);
        else if (isMissile)
            Destroy(gameObject);
    }
}