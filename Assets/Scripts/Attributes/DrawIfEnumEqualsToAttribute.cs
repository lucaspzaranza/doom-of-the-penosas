using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DrawIfEnumEqualsToAttribute : PropertyAttribute
{
    public string currentPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public object enumToCompare { get; private set; }
    public bool elseDrawItDisabled { get; private set; }

    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    /// <param name="currentPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    public DrawIfEnumEqualsToAttribute(string currentPropertyName, object enumToCompare, object comparedValue, bool elseDrawItDisabled = false)
    {        
        this.currentPropertyName = currentPropertyName;
        this.enumToCompare = enumToCompare;
        this.comparedValue = comparedValue;
        this.elseDrawItDisabled = elseDrawItDisabled;
    }
}
