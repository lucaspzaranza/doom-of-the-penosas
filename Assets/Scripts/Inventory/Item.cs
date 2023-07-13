using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float overlapRadius = 0.05f;

    private Rigidbody2D currentRigidBody2D = null;
    private bool isGrounded;

    private void Start()
    {
        currentRigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, overlapRadius, terrainLayerMask);

        if(isGrounded && currentRigidBody2D?.bodyType != RigidbodyType2D.Static)
            currentRigidBody2D.bodyType = RigidbodyType2D.Static;
        else if(!isGrounded && currentRigidBody2D?.bodyType != RigidbodyType2D.Dynamic)
            currentRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    public abstract void GetItem(Penosa player);

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == ConstantStrings.PlayerTag)
        {
            Penosa player = other.GetComponent<Penosa>();
            GetItem(player);
            Destroy(gameObject);
        }
    }
}