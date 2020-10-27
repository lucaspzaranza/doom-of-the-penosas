using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kawarimi : Grenade
{
    public float torque;
    [HideInInspector] public GameObject penosa;
    private Collider2D coll2D;
    public override void Start()
    {      
        ThrowGrenade(speed, Mathf.Abs(speed));
        rb2D.AddTorque(torque, ForceMode2D.Impulse);
        coll2D = GetComponent<Collider2D>();
    }

    public override void Update()
    {
        if(Input.GetButtonDown("Fire2")) KawarimiNoJutsu();
        if(rb2D.velocity == Vector2.zero)
        {
            if(!coll2D.isTrigger) coll2D.isTrigger = true;
            rb2D.bodyType = RigidbodyType2D.Static;
        }
    }

    private void KawarimiNoJutsu()
    {
        if(penosa != null) penosa.transform.position = transform.position;
        Destroy(gameObject);
    }
}