using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : SpecialItem
{
    public const float defaultSpeedEnhancingRate = 2f;

    private float speedEnhancingRate = defaultSpeedEnhancingRate;
    [SerializeField] private byte _duration = defaultDuration;
    public byte Duration => _duration;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<Adrenaline>();
    }

    void Update()
    {
        if(ItemInUse)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= Duration)
            {
                timeCounter = 0;
                parentSlot.Player.speed = PlayerConsts.defaultSpeed;
                ItemInUse = false;
                RemoveItemIfAmountEqualsZero();
            }
        }
    }

    public override void Use()
    {
        base.Use();
        ItemInUse = true;
        parentSlot.Player.speed *= speedEnhancingRate;
    }
}