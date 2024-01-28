using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JetSkinha : RideArmor
{    
    [SerializeField] private float _speed;
    [SerializeField] private float _playerEjectOffset;
    [SerializeField] private float _ejectForce;
    [SerializeField] private ParticleSystem _particleSystem;

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
        {
            if(_particleSystem.isEmitting)
                _particleSystem.Stop();
            return;
        }

        transform.Translate(direction * _speed * Time.deltaTime);

        if(direction.x != 0)
        {
            _particleSystem.Play();
            if(direction.x > 0)
                _particleSystem.transform.rotation = Quaternion.identity;
            else
                _particleSystem.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        else if (_particleSystem.isEmitting)
            _particleSystem.Stop();
    }    

    public override void Blink(Color newColor)
    {
        base.Blink(newColor);
        _equippedPlayer.GetComponent<SpriteRenderer>().color = newColor;
    }
}
