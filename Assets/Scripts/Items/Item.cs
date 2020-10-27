using UnityEngine;

public abstract class Item : MonoBehaviour
{ 
    public abstract void GetItem(Penosa player);
   
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Penosa player = other.gameObject.GetComponent<Penosa>();
            GetItem(player);
            Destroy(gameObject);
        }
    }
}