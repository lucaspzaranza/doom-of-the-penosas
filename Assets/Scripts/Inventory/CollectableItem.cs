using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    //[SerializeField] private MonoScript itemScript;
    [SerializeField] private SpecialItemType itemType;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private byte amount = 1;

    private Rigidbody2D currentRigidBody2D = null;
    private bool isGrounded;

    private void Start()
    {
        currentRigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, terrainLayerMask);

        if (isGrounded && currentRigidBody2D?.bodyType != RigidbodyType2D.Static)
            currentRigidBody2D.bodyType = RigidbodyType2D.Static;
        else if (!isGrounded && currentRigidBody2D?.bodyType != RigidbodyType2D.Dynamic)
            currentRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    private void GetItem(Penosa player)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;

        player.Inventory.AddItem(itemType, amount, player, sprite);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(ConstantStrings.PlayerTag))
        {
            var player = other.gameObject.GetComponent<Penosa>();
            GetItem(player);
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag(ConstantStrings.RideArmorTag))
        {
            var rideArmor = other.gameObject.GetComponent<RideArmor>();
            GetItem(rideArmor.Player);
            Destroy(gameObject);
        }
    }
}