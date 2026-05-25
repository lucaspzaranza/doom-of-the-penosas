using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ItemSlot: IItemSlot
{
    [SerializeField] private SpecialItem _item;
    [SerializeField] private byte _amount;
    [SerializeField] private Sprite _sprite;

    public IPlayerCharacter Player {get; set;}

    public const byte maxItemAmount = 99;

    public ISpecialItem Item
    {
        get { return _item; }
        set 
        {
            _item = value as SpecialItem; 
            _item.SetSlot(this);
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

    public ItemSlot(SpecialItem specialItem, byte amount, IPlayerCharacter currentPlayer, Sprite newSprite = null)
    {
        Item = specialItem;
        Amount = amount;
        Player = currentPlayer;

        if (newSprite != null)
            Sprite = newSprite;
    }
}