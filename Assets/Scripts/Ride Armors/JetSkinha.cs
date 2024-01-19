using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JetSkinha : RideArmor
{    
    [SerializeField] private float _speed;
    [SerializeField] private float _playerEjectOffset;
    [SerializeField] private float _ejectForce;    

    public override void Equip(Penosa player, PlayerController playerController)
    {
        base.Equip(player, playerController);        
    }

    public override void Eject()
    {
        SetEquippedPlayerActivation(Player, false);
        Player.Rigidbody2D.AddForce(Vector2.up * _ejectForce);
        Player.transform.position = new Vector2(transform.position.x, 
            transform.position.y + _playerEjectOffset);

        base.Eject();
    }

    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        if (SharedFunctions.HitWall(_wallCheckCollider, _terrainLayerMask, out Collider2D hitWall))
            return;

        //RigiBody2DComponent.velocity = new Vector2(direction.x, RigiBody2DComponent.velocity.y);
        transform.Translate(direction * _speed * Time.deltaTime);
    }    

    public override void Blink(Color newColor)
    {
        base.Blink(newColor);
        _equippedPlayer.GetComponent<SpriteRenderer>().color = newColor;
    }
}
