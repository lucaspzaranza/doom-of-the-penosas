using UnityEngine;

public interface IItemSlot
{
    ISpecialItem Item { get; set; }
    byte Amount { get; set; }
    public Sprite Sprite { get; set; }
}