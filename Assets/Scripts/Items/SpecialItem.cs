using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{
    public const byte defaultDuration = 30;
    public bool ItemInUse { get; protected set; }
    public ItemSlot parentSlot;

    private Rigidbody2D currentRigidBody2D = null;
    private bool isGrounded;

    [SerializeField] protected float timeCounter;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask terrainLayerMask;

    public static event EventHandler<SpriteAddedEventArgs> SpriteAdded;
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

    public abstract void GetItem<T>(Penosa player);

    public virtual void Use()
    {
        parentSlot.Player.Inventory.DecreaseItemAmount(parentSlot);
    }

    protected void RemoveItemIfAmountEqualsZero()
    {
        if (parentSlot.Player.Inventory.SelectedSlot.Amount == 0)
            parentSlot.Player.Inventory.RemoveItem(parentSlot);
    }

    protected virtual void OnSpriteAdded(SpriteAddedEventArgs e)
    {
        var handler = SpriteAdded;
        handler?.Invoke(this, e);
    }

    private void AddSpriteOnInventory(Penosa player)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        SpriteAddedEventArgs e = new SpriteAddedEventArgs(sprite);

        OnSpriteAdded(e);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            var player = other.gameObject.GetComponent<Penosa>();
            AddSpriteOnInventory(player);
            GetItem<SpecialItem>(player);
            if (player.Inventory.Slots.Count == 1) player.Inventory.ShowSlot();
            Destroy(gameObject);
        }
    }
}