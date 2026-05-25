using SharedData.Enumerations;
using UnityEngine;

public interface IInventoryListItem
{
    SpecialItemType SpecialItemType { get; set; }
    byte Amount { get; set; }
    Sprite ItemSprite { get; set; }
}