using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWithCursor : MonoBehaviour
{
    public static Action OnMenuDisabled;
    public static Action OnMenuEnabled;

    private void OnEnable()
    {
        OnMenuEnabled?.Invoke();
    }

    private void OnDisable()
    {
        OnMenuDisabled?.Invoke();
    }
}
