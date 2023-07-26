using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{
    public bool ItemInUse { get; protected set; }

    protected float timeCounter;
    protected Penosa Player => _player;
    protected Inventory PlayerInventory => _inventory;
    public ItemSlot ItemSlot => _itemSlot;
    public SpecialItemType ItemType => _type;

    [SerializeField]
    protected SpecialItemType _type;

    private Penosa _player;
    private Inventory _inventory;
    private ItemSlot _itemSlot;

    private void Awake()
    {
        _player = GetComponentInParent<Penosa>();
        _inventory = GetComponent<Inventory>();
    }

    public abstract void GetPlayerValues();

    public virtual void Use()
    {
        PlayerInventory.DecreaseItemAmount(ItemSlot);
    }

    public void SetSlot(ItemSlot slotToSet)
    {
        if(_itemSlot == null)
            _itemSlot = slotToSet;
    }

    public void RemoveItemIfAmountEqualsZero()
    {
        if (PlayerInventory.SelectedSlot.Amount == 0)
            PlayerInventory.RemoveItem(ItemSlot);
    }
}