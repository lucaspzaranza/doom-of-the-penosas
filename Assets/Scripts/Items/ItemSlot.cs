using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private SpecialItem _item;
    [SerializeField] private byte _amount;
    public Penosa Player {get; set; }

    public SpecialItem Item
    {
        get { return _item; }
        set { _item = value; }        
    }

    public byte Amount
    {
        get { return _amount; }
        set { _amount = value; } 
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