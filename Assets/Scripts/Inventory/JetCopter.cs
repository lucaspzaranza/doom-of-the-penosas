using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JetCopter : SpecialItem
{
    private byte maxDuration;

    void Update()
    {
        if(ItemInUse)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= maxDuration)
            {
                timeCounter = 0;
                SetJetCopterActivation(false);
                RemoveItemIfAmountEqualsZero();
            }
        }
    }

    public void SetJetCopterActivation(bool value)
    {
        ItemSlot.Player.JetCopterObject.SetActive(value);
        ItemSlot.Player.JetCopterActivated = value;
        ItemSlot.Player.Animator.SetBool(ConstantStrings.JetCopter, value);

        // Se true, coloca uma gravidade menor, senão, a gravidade normal
        ItemSlot.Player.GetComponent<Rigidbody2D>().gravityScale = 
            value? ItemSlot.Player.Inventory.JetCopterGravity : PlayerConsts.DefaultGravity;
        ItemInUse = value;
    }

    public override void Use()
    {
        GetPlayerValues();
        base.Use();
        SetJetCopterActivation(true);
    }

    public override void GetPlayerValues()
    {
        maxDuration = PlayerInventory.ItemEffectDuration;
    }
}