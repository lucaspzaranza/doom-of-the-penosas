using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfEnumEqualsToAttribute))]
public class DrawIfEnumEqualsPropertyDrawer : PropertyDrawer
{
    /// <summary>
    /// Reference to the attribute on the property.
    /// </summary>
    DrawIfEnumEqualsToAttribute _drawIfEnumAttribute;

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
        _drawIfEnumAttribute = attribute as DrawIfEnumEqualsToAttribute;
        _comparedField = property.serializedObject.FindProperty(_drawIfEnumAttribute.currentPropertyName);

        // Get the value of the compared field.
        int currentFieldValueIndex = _comparedField.enumValueIndex;
        object enumToCompare = _drawIfEnumAttribute.enumToCompare;

        Array enumValues = Enum.GetValues(enumToCompare.GetType());
        string currentEnumValue = enumValues.GetValue(currentFieldValueIndex).ToString();
        string enumValueToCompare = _drawIfEnumAttribute.comparedValue.ToString();

        // Is the condition met? Should the field be drawn?
        return currentEnumValue == enumValueToCompare;
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
            if (_drawIfEnumAttribute.elseDrawItDisabled)
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
