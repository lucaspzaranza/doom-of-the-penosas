using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTalk : SpecialItem
{
    public static Func<bool> OnRequestWalkTalkUse;
    public static Action<byte> OnWalkTalk;

    public override void GetPlayerValues()
    {
        
    }

    public override void Use()
    {
        bool canUseWalkTalk = (OnRequestWalkTalkUse == null)? 
            false : OnRequestWalkTalkUse.Invoke();

        if (canUseWalkTalk)
        {
            base.Use();
            OnWalkTalk?.Invoke(Player.PlayerData.LocalID);
            RemoveItemIfAmountEqualsZero();
        }
        else
            Debug.LogWarning("Seems like one or more players are using Ride Armors. " +
                "Please eject the armors to use the Walk Talk.");
    }
}
