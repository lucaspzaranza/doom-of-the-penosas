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
public class CutSceneSO : TextSOBase<CutSceneStep>
{
}

[System.Serializable]
public class DialogBoxText
{
    [SerializeField] private string _name;

    [SerializeField] private Language _language;
    public Language Language => _language;

    [TextArea]
    [SerializeField] private string _text;
    public string Text => _text;
}

[System.Serializable]
public class CutSceneStep: Step
{    
    [SerializeField] private bool _useVideoInsteadSprite;
    public bool UseVideoInsteadSprite => _useVideoInsteadSprite;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private VideoClip _videoClip;
    public VideoClip VideoClip => _videoClip;    
}
