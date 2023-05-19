using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods
{
    /// <summary>
    /// Returns the GameObject with the cursor position.
    /// Should be used only in Button GameObjects.
    /// </summary>
    /// <param name="button">The UI Button to get the cursor.</param>
    /// <returns>The CursorPosition GameObject.</returns>
    public static GameObject GetCursorPositionGameObject(this GameObject button)
    {
        if (button == null || !button.TryGetComponent(typeof(Button), out Component buttonComponent))
        {
            Debug.LogWarning($"The Button GameObject hasn't any Button Component, and he may not behave properly.");
            return null;
        }

        GameObject child = button.transform.Find(ConstantStrings.CursorPositionName)?.gameObject;

        return child;
    }

    /// <summary>
    /// Returns the cursor position the cursor must be positioned.
    /// Should be used only in Button GameObjects.
    /// </summary>
    /// <param name="button">The UI Button to get the cursor.</param>
    /// <returns>The CursorPosition GameObject. Returns a Vector3.Zero if can't find any Button or CursorPosition object.</returns>
    public static Vector3 GetCursorPosition(this GameObject button)
    {
        if (button == null || !button.TryGetComponent(typeof(Button), out Component buttonComponent))
        {
            Debug.LogWarning($"The Button GameObject hasn't any Button Component, and he may not behave properly.");
            return Vector3.zero;
        }

        GameObject child = button.transform.Find(ConstantStrings.CursorPositionName)?.gameObject;

        if (child != null)
        {
            Vector3 position = child.GetComponent<RectTransform>().localPosition;
            return position;
        }
        else
            return Vector3.zero;
    }
}
