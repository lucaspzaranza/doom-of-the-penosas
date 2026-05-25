using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : SpecialItem
{
    private float speedEnhancingRate;
    private byte duration;

    public override void GetPlayerValues()
    {
        duration = PlayerInventory.ItemEffectDuration;
        speedEnhancingRate = PlayerInventory.AdrenalineSpeedEnhancingRate;
    }
   
    void Update()
    {
        if(ItemInUse)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= duration)
            {
                timeCounter = 0;
                Player._speed = PlayerConsts.DefaultSpeed;
                ItemInUse = false;
                RemoveItemIfAmountEqualsZero();
            }
        }
    }

    public override void Use()
    {
        GetPlayerValues();
        base.Use();
        ItemInUse = true;
        Player._speed *= speedEnhancingRate;
    }
}