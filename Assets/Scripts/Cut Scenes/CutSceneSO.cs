using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CutScene", menuName = "ScriptableObjects/CutScenes")]
public class CutSceneSO : ScriptableObject
{
    [SerializeField] private bool _skippable;
    public bool Skippable => _skippable;

    [SerializeField] private List<CutSceneStep> _steps;
    public IReadOnlyList<CutSceneStep> Steps => _steps;

    private int _stepCounter;

    private void OnEnable()
    {
        _stepCounter = 0;
    }

    private void OnDisable()
    {
        _stepCounter = 0;
    }

    public void NextStep()
    {
        _stepCounter++;
    }

    public void ShowStep(int index)
    {

    }

    public void Skip()
    {

    }
}

[System.Serializable]
public class CutSceneStep
{
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

    [TextArea]
    [SerializeField] private string _text;
    public string Text => _text;
}
