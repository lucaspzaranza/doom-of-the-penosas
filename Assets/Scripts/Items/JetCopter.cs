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

    private void SetJetCopterActivation(bool value)
    {
        itemSlot.Player.JetCopterObject.SetActive(value);
        itemSlot.Player.JetCopterActivated = value;
        itemSlot.Player.Animator.SetBool("JetCopter", value);

        // Se true, coloca uma gravidade menor, senão, a gravidade normal
        itemSlot.Player.GetComponent<Rigidbody2D>().gravityScale = 
            value? itemSlot.Player.Inventory.JetCopterGravity : itemSlot.Player.defaultGravity;
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
        maxDuration = Inventory.ItemEffectDuration;
    }
}