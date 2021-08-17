using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JetCopter : SpecialItem
{
    public byte maxDuration = defaultDuration;
    [SerializeField] private float gravity = 0.1f;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<JetCopter>();
        player.Inventory.GetComponent<JetCopter>().gravity = gravity;
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

        // Se true, coloca uma gravidade menor, senão, a gravidade normal
        parentSlot.Player.GetComponent<Rigidbody2D>().gravityScale = 
            value? gravity : parentSlot.Player.defaultGravity;
        ItemInUse = value;
    }

    public override void Use()
    { 
        base.Use();
        SetJetCopterActivation(true);
    }
}