using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JetCopter : SpecialItem
{
    public byte maxDuration = defaultDuration;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<JetCopter>();
    }

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
        parentSlot.Player.JetCopterObject.SetActive(value);
        parentSlot.Player.JetCopterActivated = value;
        parentSlot.Player.Animator.SetBool("JetCopter", value);

        // Se true, coloca o dobro da velocidade do paraquedas, senão, a gravidade normal
        parentSlot.Player.GetComponent<Rigidbody2D>().gravityScale = 
            value? parentSlot.Player.parachuteGravity * 2 : parentSlot.Player.defaultGravity;
        ItemInUse = value;
    }

    public override void Use()
    { 
        base.Use();
        SetJetCopterActivation(true);
    }
}