using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JetCopter : SpecialItem
{
    private const byte seconds = 30;
    public byte maxDuration = seconds;
    public float timeCounter;
    private bool active;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<JetCopter>();
    }

    void Update()
    {
        if(active)
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
        parentSlot.Player.JetCopterObject.SetActive(value);
        parentSlot.Player.JetCopterActivated = value;
        parentSlot.Player.Animator.SetBool("JetCopter", value);
        active = value;
    }

    public override void Use()
    { 
        base.Use();
        SetJetCopterActivation(true);
    }
}