using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{    
    public const byte defaultDuration = 30;
    public bool ItemInUse {get; protected set;}
    public ItemSlot parentSlot;
    [SerializeField] protected float timeCounter;
    public static event EventHandler<SpriteAddedEventArgs> SpriteAdded;

    public abstract void GetItem<T>(Penosa player);

    public virtual void Use()
    {
        parentSlot.Player.Inventory.DecreaseItemAmount(parentSlot);
    }

    protected void RemoveItemIfAmountEqualsZero()
    {
        if(parentSlot.Player.Inventory.SelectedSlot.Amount == 0) 
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

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {            
            var player = other.gameObject.GetComponent<Penosa>();                      
            AddSpriteOnInventory(player);
            GetItem<SpecialItem>(player);
            if(player.Inventory.Slots.Count == 1) player.Inventory.ShowSlot();
            Destroy(gameObject);
        }
    }
}