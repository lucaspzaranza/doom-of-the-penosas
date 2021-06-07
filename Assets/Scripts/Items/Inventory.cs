using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Inventory : MonoBehaviour
{
    #region Vars & Props
    [SerializeField] private SpecialItem _selectedItem;
    private ItemSlot _selectedSlot = null;
    public ItemSlot SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;
            _selectedItem = _selectedSlot == null ? null : _selectedSlot.Item;
        }
    }
    [SerializeField] private List<ItemSlot> _slots = null;
    public List<ItemSlot> Slots { get => _slots; }
    public bool IsEmpty => Slots.Count == 0;

    // Sprite Add Event data
    public List<Sprite> itemSprites;
    private Sprite currentItemSprite;
    [SerializeField] private SpriteRenderer itemChildSR = null;
    public TextMeshPro itemAmount;
    public float slotTemporizer;
    public GameObject slotGameObject;
    private float timeCounter;

    private Penosa player;
    #endregion

    void Start()
    {
        SpecialItem.SpriteAdded += SpriteAdded;
        player = GetComponentInParent<Penosa>();
    }

    void Update()
    {
        if (slotGameObject.activeSelf)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= slotTemporizer)
            {
                timeCounter = 0;
                slotGameObject.SetActive(false);
            }
        }
    }

    public void ClearInventory()
    {
        var items = new List<SpecialItem>(GetComponents<SpecialItem>());
        Slots.RemoveRange(0, Slots.Count);
        itemSprites.RemoveRange(0, itemSprites.Count);
        items.ForEach(item => Destroy(item));
        SelectedSlot = null;
        SelectSlotSprite(null);
        itemAmount.text = "0";
    }

    public void ShowSlot()
    {
        slotGameObject.SetActive(true);
        timeCounter = 0;
    }

    public void AddItem<T>() where T : SpecialItem, new()
    {
        var matchSlot =
            Slots.Find(slot => slot.Item == null || slot.Item.GetType() == typeof(T));

        if (matchSlot != null)
            SetItemAmount(matchSlot, (byte)(matchSlot.Amount + 1));
        else
        {
            T newItem = gameObject.AddComponent<T>();
            var newSlot = new ItemSlot(newItem, player);
            Slots.Add(newSlot);
            if (Slots.Count == 1) SelectedSlot = newSlot;
            SetItemAmount(newSlot, 1);
        }
    }

    private void SetItemAmount(ItemSlot item, byte amount)
    {
        item.Amount = amount;
        if (item.Equals(SelectedSlot))
            itemAmount.text = amount.ToString();
    }

    public void DecreaseItemAmount(ItemSlot slot)
    {
        SetItemAmount(slot, (byte)(slot.Amount - 1));
    }

    public void RemoveItem(ItemSlot slot)
    {
        int index = Slots.IndexOf(slot);
        Slots.Remove(slot);
        itemSprites.RemoveAt(index);
        Destroy(slot.Item);
        int newIndex = index == Slots.Count ? index - 1 : index;
        SelectedSlot = IsEmpty ? null : Slots[newIndex];
        SelectSlotSprite(IsEmpty ? null : itemSprites[newIndex]);
        itemAmount.text = IsEmpty ? "0" : SelectedSlot.Amount.ToString();
    }

    public void SelectItem(int index)
    {
        SelectedSlot = Slots[index];
        SelectSlotSprite(itemSprites[index]);
        itemAmount.text = SelectedSlot.Amount.ToString();
    }

    private void SpriteAdded(object sender, SpriteAddedEventArgs e)
    {
        if (!itemSprites.Contains(e.NewSprite))
            itemSprites.Add(e.NewSprite);

        if (itemSprites.Count == 1)
            SelectSlotSprite(itemSprites[0]);
    }

    public void SelectSlotSprite(Sprite newSprite)
    {
        currentItemSprite = newSprite;
        itemChildSR.sprite = newSprite;

        player.HUD.SetSpecialItemIconSprite(currentItemSprite);
    }
}