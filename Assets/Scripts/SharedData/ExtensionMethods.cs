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
            WarningMessages.ButtonComponentNotFound();
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
        if (button == null || !button.TryGetComponent(out Button buttonComponent))
        {
            WarningMessages.ButtonComponentNotFound();
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

    public static bool IsDeviceSelectionButton(this GameObject button)
    {
        if (button == null || !button.TryGetComponent(out Button buttonComponent))
        {
            WarningMessages.ButtonComponentNotFound();
            return false;
        }

        return button.tag == ConstantStrings.DeviceSelectionButtonTag;
    }

    public static bool IsCharacterSelectionButton(this GameObject button)
    {
        if (button == null || !button.TryGetComponent(out Button buttonComponent))
        {
            WarningMessages.ButtonComponentNotFound();
            return false;
        }

        return button.tag == ConstantStrings.CharacterSelectionButtonTag;
    }

    /// <summary>
    /// Performs a loop into all object parents and returns the first <strong>T</strong> Component found. 
    /// </summary>
    /// <returns>If found nothing, returns <strong>false</strong> and <strong>null</strong> at the <i>foundComponent</i> param.
    /// <br/>Otherwise, returns <strong>true</strong> 
    /// and the <i>foundComponent</i> instance.</returns>
    public static bool GetComponentInAnyParent<T>(this MonoBehaviour thisObject, out T foundComponent)
    {
        foundComponent = default(T);

        GameObject parent = thisObject.transform.parent?.gameObject;

        while (!parent.TryGetComponent(out foundComponent))
        {
            parent = parent.transform.parent?.gameObject;
            if (parent == null)
            {
                Debug.LogWarning($"{typeof(T)} Component not found in any parent.");
                return false;
            }
        }

        return true;
    }
}
