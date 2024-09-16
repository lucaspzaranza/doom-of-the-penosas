using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPlayerDetector : MonoBehaviour
{
    public Action OnPlayerTriggerExit;

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == ConstantStrings.PlayerTag)
        {
            print("OnCollisionExit2D");
            OnPlayerTriggerExit?.Invoke();
        }
    }
}
