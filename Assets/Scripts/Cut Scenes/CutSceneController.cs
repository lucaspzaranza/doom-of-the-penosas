using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public VideoPlayer VideoPlayerProp => _videoPlayer;

    [SerializeField] private GameObject _videoPlayerGameObject;
    public GameObject VideoPlayerGameObject => _videoPlayerGameObject;

    //[DrawItDisabled, SerializeField] private TextMeshProUGUI _stepText;
    //public TextMeshProUGUI StepText => _stepText;

    [DrawItDisabled, SerializeField] private DialogBox _dialogBox;
    public DialogBox DialogBox => _dialogBox;

    [SerializeField] private GameObject _fadeInOut;
    public GameObject FadeInOut => _fadeInOut;

    private bool _canNextStep;
    public bool CanNextStep => _canNextStep;

    private string CurrentSceneText => _currentStep.GetText(_gameCtrlInstance.Language);

    private CutSceneControllerBackup _cutSceneBackup;
    private CutSceneStep _currentStep;
    //private bool _showTextInProgress;
    private string _currentText;
    private float _timeCounter;
    private int _charIndex;
    private GameController _gameCtrlInstance;

    public override void Setup()
    {
        CutSceneSO introCutScene = CutScenes.FirstOrDefault(cutScene => cutScene.name.Contains("Intro"));
        SetCutScene(introCutScene);

        Step.OnStepInitialized += HandleOnStepInitialized;
        DialogBox.OnNextButtonPressed+= HandleOnNextStepButtonPressed;
        DialogBox.OnSkipButtonPressed += HandleOnCutSceneSkip;
        _currentText = string.Empty;

        NextStepAnimationEvent.OnNextStepAnimationEvent += ShowNextStep;

        _canNextStep = true;
        _gameCtrlInstance = TryToGetGameControllerFromParent();
    }

    public override void Dispose()
    {
        SetCutScene(null);
        DialogBox.OnSkipButtonPressed -= HandleOnCutSceneSkip;
        DialogBox.OnNextButtonPressed -= HandleOnNextStepButtonPressed;
        Step.OnStepInitialized -= HandleOnStepInitialized;

        NextStepAnimationEvent.OnNextStepAnimationEvent -= ShowNextStep;
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        CutSceneControllerBackup cutSceneBackup = backup as CutSceneControllerBackup;
        _cutSceneBackup = cutSceneBackup;

        _videoPlayer = cutSceneBackup.VideoPlayer;
        _image = cutSceneBackup.Image;
        _dialogBox = cutSceneBackup.DialogBox;
        _fadeInOut = cutSceneBackup.FadeInOut;
        _videoPlayerGameObject = cutSceneBackup.VideoPlayerGameObject;

        if (!_canNextStep)
            _canNextStep = true;

        PlayCutScene();
    }

    public void PlayCutScene()
    {
        if (CurrentCutscene != null)
        {
            DialogBox.SetTextSO(CurrentCutscene);
            DialogBox.PlayStep(0);
        }
    }

    private void ShowNextStep()
    {
        if (_currentStep.UseVideoInsteadSprite)
        {
            VideoPlayerProp.Stop();
            VideoPlayerProp.frame = 0;
        }

        DialogBox.NextStep();
        _canNextStep = true;
    }

    private void HandleOnNextStepButtonPressed()
    {
        if (!DialogBox.ShowTextInProgress)
        {
            // The Fade-In-Out animation triggers the ShowNextStep() by default.
            if (DialogBox.StepCounter < CurrentCutscene.Steps.Count - 1)
            {
                _canNextStep = false;
                FadeInOut.SetActive(false);
                FadeInOut.SetActive(true);
            }
            else if (DialogBox.StepCounter == CurrentCutscene.Steps.Count - 1)
            {
                // Go to the Next Step bypassing any fade animation.
                // Only used when you are at the last step which calls the Cut Scene skip.
                ShowNextStep(); 
            }
        }
    }

    private void HandleOnStepInitialized(Step step)
    {
        _currentStep = step as CutSceneStep;

        if (_currentStep.UseVideoInsteadSprite) 
        {
            Image.gameObject.SetActive(false);
            VideoPlayerGameObject.SetActive(true);
            VideoPlayerProp.renderMode = VideoRenderMode.RenderTexture;
            VideoPlayerProp.clip = _currentStep.VideoClip;
            VideoPlayerProp.Play();
        }
        else
        {
            if (_currentStep.Sprite != null)
            {
                VideoPlayerGameObject.SetActive(false);
                Image.gameObject.SetActive(true);
                Image.sprite = _currentStep.Sprite;
            }
            else
                Image.gameObject.SetActive(false);
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
