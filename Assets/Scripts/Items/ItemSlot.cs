using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [SerializeField] private SpecialItem _item;
    [SerializeField] private byte _amount;
    [SerializeField] private Sprite _sprite;

    public Penosa Player {get; set;}

    public const byte maxItemAmount = 99;

    public SpecialItem Item
    {
        get { return _item; }
        set 
        {
            _item = value; 
            _item.itemSlot = this;
        }        
    }

    public byte Amount
    {
        get { return _amount; }
        set { _amount = (byte)Mathf.Clamp(value, 0, maxItemAmount); } 
    }


    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public ItemSlot(SpecialItem specialItem, byte amount, Penosa currentPlayer, Sprite newSprite = null)
    {
        Item = specialItem;
        Amount = amount;
        Player = currentPlayer;
        if (newSprite != null)
            Sprite = newSprite;
    }

    public ItemSlot(SpecialItem specialItem, byte amount, Penosa currentPlayer)
    {
        Item = specialItem;
        Player = currentPlayer;
        Amount = amount;
    }

    public ItemSlot(SpecialItem specialItem, Penosa currentPlayer)
    {
        Item = specialItem;
        Player = currentPlayer;
    }
}