using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{    
    public ItemSlot parentSlot;

    public static event EventHandler<SpriteAddedEventArgs> SpriteAdded;

    public abstract void GetItem<T>(Penosa player);

    public abstract void Use();

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