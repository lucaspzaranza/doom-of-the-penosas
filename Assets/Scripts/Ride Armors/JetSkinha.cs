using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JetSkinha : RideArmor
{    
    [SerializeField] private float _speed;
    [SerializeField] private float _playerEjectOffset;
    [SerializeField] private float _ejectForce;
    [SerializeField] private ParticleSystem _frontParticleSystem;
    [SerializeField] private ParticleSystem _backParticleSystem;

    private bool _canMove = false;
    private bool _requiredSinceTheBeginning;

    private void Start()
    {
        _requiredSinceTheBeginning = _required;
    }

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

        StopWaterParticleEmission();

        base.Eject();
    }

    private void StartWaterParticleEmission()
    {
        _backParticleSystem.Play();
        _frontParticleSystem.Play();
    }

    private void StopWaterParticleEmission()
    {
        if (_backParticleSystem.isEmitting)
            _backParticleSystem.Stop();

        if (_frontParticleSystem.isEmitting)
            _frontParticleSystem.Stop();
    }

    private void SetWaterParticleRotation(Quaternion rotation)
    {
        _frontParticleSystem.transform.rotation = rotation;
        _backParticleSystem.transform.rotation = rotation;
    }

    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        if (!_canMove || SharedFunctions.HitWall(_wallCheckCollider, _terrainWithoutPlatformLayerMask, out Collider2D hitWall))
        {
            StopWaterParticleEmission();
            return;
        }

        transform.Translate(direction * _speed * Time.deltaTime);

        if(direction.x != 0)
        {            
            StartWaterParticleEmission();
            if (direction.x > 0)
                SetWaterParticleRotation(Quaternion.identity);
            else
                SetWaterParticleRotation(new Quaternion(0f, 180f, 0f, 0f));
        }
        else if (_backParticleSystem.isEmitting)
        {
            StopWaterParticleEmission();
        }
    }    

    public override void Blink(Color newColor)
    {
        base.Blink(newColor);
        _equippedPlayer.GetComponent<SpriteRenderer>().color = newColor;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        _canMove = other.gameObject.layer == 4; // Water Layer

        if (!_canMove)
        {
            _armorActivator.gameObject.SetActive(false);

            if(Required)
                Required = false;
        }
        else if(_canMove)
        {
            _armorActivator.gameObject.SetActive(true);

            if(!Required && _requiredSinceTheBeginning)
                Required = true;
        }
    }
}
