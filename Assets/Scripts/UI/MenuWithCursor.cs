using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWithCursor : MonoBehaviour
{
    public static Action<List<CursorPosition>> OnMenuEnabled;
    public static Action<List<CursorPosition>> OnMenuDisabled;

    [SerializeField] private List<CursorPosition> _cursors;

    private void OnEnable()
    {
        OnMenuEnabled?.Invoke(_cursors);
    }

    private void OnDisable()
    {
        OnMenuDisabled?.Invoke(_cursors);
    }
}
