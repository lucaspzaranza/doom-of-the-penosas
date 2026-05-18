using SharedData.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Step
{
    public static Action<Step> OnStepInitialized;

    [Tooltip("The list of texts for each language used in the game for this step.")]
    [SerializeField] protected List<DialogBoxText> _stepTexts;

    [SerializeField] protected float _textSpeed;
    public float TextSpeed => _textSpeed;

    public void InitializeStep()
    {
        OnStepInitialized?.Invoke(this);
    }

    /// <summary>
    /// Get cut scene text based upon the desired language passed as parameter.
    /// </summary>
    /// <returns></returns>
    public string GetText(Language language)
    {
        return _stepTexts
            .SingleOrDefault(langTxt => langTxt.Language.Equals(language)).Text
            ??
            string.Empty;
    }
}
