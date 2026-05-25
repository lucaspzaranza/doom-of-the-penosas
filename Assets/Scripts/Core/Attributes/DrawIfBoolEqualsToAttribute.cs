using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DrawIfBoolEqualsToAttribute : PropertyAttribute
{
    public string comparedPropertyName { get; private set; }
    public bool comparedValue { get; private set; }
    public bool elseDrawItDisabled { get; private set; }

    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    public DrawIfBoolEqualsToAttribute(string comparedPropertyName, bool comparedValue, bool elseDrawItDisabled = false)
    {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
        this.elseDrawItDisabled = elseDrawItDisabled;
    }
}
