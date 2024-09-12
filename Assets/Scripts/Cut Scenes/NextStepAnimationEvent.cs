using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStepAnimationEvent : MonoBehaviour
{
    public static Action OnNextStepAnimationEvent;

    public void FireNextStepAnimationEvent()
    {
        OnNextStepAnimationEvent?.Invoke();
    }
}
