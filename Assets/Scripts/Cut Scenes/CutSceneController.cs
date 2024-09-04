using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutSceneController : ControllerUnit
{
    public Action OnCutSceneSkipRequest;

    [SerializeField] private CutSceneSO _currentCutscene;
    public CutSceneSO CurrentCutscene => _currentCutscene;

    [SerializeField] private List<CutSceneSO> _cutScenes;
    public IReadOnlyList<CutSceneSO> CutScenes => _cutScenes;

    [DrawItDisabled, SerializeField] private Image _image;
    public Image Image => _image;

    [DrawItDisabled, SerializeField] private VideoPlayer _videoPlayer;
    public VideoPlayer VideoPlayer => _videoPlayer;

    [DrawItDisabled, SerializeField] private TextMeshProUGUI _stepText;
    public TextMeshProUGUI StepText => _stepText;

    private CutSceneStep _currentStep;
    private bool _showTextInProgress;
    private string _currentText;
    private float _timeCounter;
    private int _charIndex;

    public override void Setup()
    {
        CutSceneSO introCutScene = CutScenes.FirstOrDefault(cutScene => cutScene.name.Contains("Intro"));
        SetCutScene(introCutScene);

        CutSceneStep.OnStepInitialized += HandleOnStepInitialized;
        CutSceneSO.OnCutSceneSkip += HandleOnCutSceneSkip;
        CutSceneSO.OnNextStepButtonPressed += HandleOnNextStepButtonPressed;
        _showTextInProgress = false;
        _currentText = string.Empty;
    }

    public override void Dispose()
    {
        SetCutScene(null);
        CutSceneSO.OnCutSceneSkip -= HandleOnCutSceneSkip;
        CutSceneSO.OnNextStepButtonPressed -= HandleOnNextStepButtonPressed;
        CutSceneStep.OnStepInitialized -= HandleOnStepInitialized;
    }

    private void Update()
    {
        if(!_showTextInProgress || _currentStep == null || _stepText == null ||
            string.IsNullOrEmpty(_currentStep.Text)) return;

        if (_stepText.text.Length == _currentStep.Text.Length)
        {
            ResetTextDisplayCounterData();
            _showTextInProgress = false;
        }

        _timeCounter += Time.deltaTime;
        if(_timeCounter >= _currentStep.SequenceSpeed)
        {
            _stepText.text += _currentStep.Text[_charIndex];
            _timeCounter = 0;
            _charIndex++;
        }
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        CutSceneControllerBackup cutSceneBackup = backup as CutSceneControllerBackup;

        _videoPlayer = cutSceneBackup.VideoPlayer;
        _image = cutSceneBackup.Image;
        _stepText = cutSceneBackup.CutSceneTMPro;

        if (_stepText != null)
            _stepText.text = string.Empty;
    }

    //public void LoadCutSceneMediaComponents()
    //{
    //    _videoPlayer = FindObjectOfType<VideoPlayer>();
    //    _image = FindObjectOfType<Image>();
    //    _stepText = FindObjectOfType<TextMeshProUGUI>();
    //    if(_stepText != null)
    //        _stepText.text = string.Empty;
    //}

    private void ResetTextDisplayCounterData()
    {
        _charIndex = 0;
        _timeCounter = 0;
    }

    public void PlayCutScene()
    {
        if(CurrentCutscene != null)
        {
            ResetTextDisplayCounterData();
            CurrentCutscene.PlayStep(0);
        }
    }

    public void Stop() 
    {
        ResetTextDisplayCounterData();
    }

    private void HandleOnNextStepButtonPressed()
    {
        if (_showTextInProgress)
            _stepText.text = _currentStep.Text;
        else
            _currentCutscene.NextStep();
    }

    private void HandleOnStepInitialized(CutSceneStep step)
    {
        _currentStep = step;
        _stepText.text = string.Empty;

        if (_currentStep.UseVideoInsteadSprite) 
        {
            VideoPlayer.clip = _currentStep.VideoClip;
            VideoPlayer.Play();
        }
        else
            Image.sprite = _currentStep.Sprite;

        if(!string.IsNullOrEmpty(_currentStep.Text))
        {
            _showTextInProgress = true;
        }
    }

    private void HandleOnCutSceneSkip()
    {
        OnCutSceneSkipRequest?.Invoke();
    }

    public void SetCutScene(CutSceneSO cutscene)
    {
        _currentCutscene = cutscene;
    }
}
