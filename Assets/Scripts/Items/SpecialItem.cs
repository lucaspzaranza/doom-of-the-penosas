using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{
    public bool ItemInUse { get; protected set; }
    public ItemSlot itemSlot;

    protected float timeCounter;

    private Penosa _player;
    private Inventory _inventory;

    protected Penosa Player => _player;
    protected Inventory Inventory => _inventory;

    private void Awake()
    {
        _player = GetComponentInParent<Penosa>();
        _inventory = GetComponent<Inventory>();
    }

    public abstract void GetPlayerValues();

    public virtual void Use()
    {
        Inventory.DecreaseItemAmount(itemSlot);
    }

    protected void RemoveItemIfAmountEqualsZero()
    {
        if (Inventory.SelectedSlot.Amount == 0)
            Inventory.RemoveItem(itemSlot);
    }
}