using JetBrains.Annotations;
using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CutScene", menuName = "ScriptableObjects/CutScenes")]
public class CutSceneSO : ScriptableObject
{
    public static Action OnCutSceneSkip;
    public static Action OnNextStepButtonPressed;

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private bool _skippable;
    public bool Skippable => _skippable;

    [SerializeField] private List<CutSceneStep> _steps;
    public IReadOnlyList<CutSceneStep> Steps => _steps;

    private int _stepCounter;
    public int StepCounter => _stepCounter;

    private void OnEnable()
    {
        _stepCounter = 0;
    }

    public void NextStepButtonPressed()
    {
        OnNextStepButtonPressed?.Invoke();
    }

    public void NextStep()
    {
        _stepCounter++;
        if (_stepCounter >= Steps.Count)
            Skip();
        else
            PlayStep(_stepCounter);
    }

    public void PlayStep(int index)
    {
        Steps[index].InitializeStep();
    }

    public void Skip()
    {
        //Debug.Log($"_stepCounter: {_stepCounter}, Steps.Count: {Steps.Count}");
        if(_stepCounter >= Steps.Count || Skippable)
        {
            _stepCounter = 0;
            OnCutSceneSkip.Invoke();
        }
    }
}

[System.Serializable]
public class CutSceneLanguageText
{
    [SerializeField] private string _name;

    [SerializeField] private Language _language;
    public Language Language => _language;

    [TextArea]
    [SerializeField] private string _text;
    public string Text => _text;
}

[System.Serializable]
public class CutSceneStep
{
    public static Action<CutSceneStep> OnStepInitialized;

    [SerializeField] private bool _useVideoInsteadSprite;
    public bool UseVideoInsteadSprite => _useVideoInsteadSprite;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private VideoClip _videoClip;
    public VideoClip VideoClip => _videoClip;

    [SerializeField] private bool _showTextInSequence;
    public bool ShowTextInSequence => _showTextInSequence;

    [SerializeField] private float _sequenceSpeed;
    public float SequenceSpeed => _sequenceSpeed;

    [Tooltip("The list of texts for each language used in the game for this step.")]
    [SerializeField] private List<CutSceneLanguageText> _stepTexts;

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
