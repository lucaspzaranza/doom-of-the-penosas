using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWithCursor : MonoBehaviour
{
    private const float eventDelay = 1f;

    public static Action<IReadOnlyList<CursorPosition>> OnMenuEnabled;
    public static Action OnMenuDisabled;

    [SerializeField] private List<CursorPosition> _cursors;
   
    private void OnEnable()
    {
        OnMenuEnabled?.Invoke(_cursors);
    }

    private void OnDisable()
    {
        OnMenuDisabled?.Invoke();
    }
}
