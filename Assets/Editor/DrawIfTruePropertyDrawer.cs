using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfBoolEqualsToAttribute))]
public class DrawIfTruePropertyDrawer : PropertyDrawer
{
    /// <summary>
    /// Reference to the attribute on the property.
    /// </summary>
    DrawIfBoolEqualsToAttribute _drawIfAttribute;

    /// <summary>
    /// Field that is being compared.
    /// </summary>
    SerializedProperty _comparedField;

    private float _propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return IsConditionMet(property) ? EditorGUI.GetPropertyHeight(property) : _propertyHeight;
    }

    private bool IsConditionMet(SerializedProperty property)
    {
        // Set the global variables.
        _drawIfAttribute = attribute as DrawIfBoolEqualsToAttribute;
        _comparedField = property.serializedObject.FindProperty(_drawIfAttribute.comparedPropertyName);

        // Get the value of the compared field.
        bool comparedFieldValue = _comparedField.boolValue;

        // Is the condition met? Should the field be drawn?
        return comparedFieldValue == _drawIfAttribute.comparedValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool conditionMet = IsConditionMet(property);

        // The height of the property should be defaulted to the default height.
        _propertyHeight = base.GetPropertyHeight(property, label);

        // If the condition is met, simply draw the field. Else...
        if (conditionMet)
        {
            EditorGUI.PropertyField(position, property, true);
        }
        else
        {
            if (_drawIfAttribute.elseDrawItDisabled)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property);
                GUI.enabled = true;
                return;
            }
            else
                _propertyHeight = 0f;
        }
    }
}
