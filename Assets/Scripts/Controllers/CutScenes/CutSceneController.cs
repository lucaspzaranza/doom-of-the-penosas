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

    [SerializeField] private DialogTrigger _dialogTrigger;
    public DialogTrigger DialogTrigger => _dialogTrigger;


    [DrawItDisabled, SerializeField] private DialogBox _dialogBox;

    public DialogBox DialogBox => _dialogBox;

    [SerializeField] private GameObject _fadeInOut;
    public GameObject FadeInOut => _fadeInOut;

    private string CurrentSceneText => _currentStep.GetText(_gameCtrlInstance.Language);

    private CutSceneControllerBackup _cutSceneBackup;
    private CutSceneStep _currentStep;
    private GameController _gameCtrlInstance;

    public override void Setup()
    {
        CutSceneSO introCutScene = CutScenes.FirstOrDefault(cutScene => cutScene.name.Contains("Intro"));
        SetCutScene(introCutScene);

        _gameCtrlInstance = TryToGetGameControllerFromParent();
    }

    public override void Dispose()
    {
        SetCutScene(null);        
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        CutSceneControllerBackup cutSceneBackup = backup as CutSceneControllerBackup;
        _cutSceneBackup = cutSceneBackup;

        _videoPlayer = cutSceneBackup.VideoPlayer;
        _image = cutSceneBackup.Image;
        _dialogTrigger = cutSceneBackup.DialogTrigger;
        _dialogBox = cutSceneBackup.DialogBox;
        _fadeInOut = cutSceneBackup.FadeInOut;
        _videoPlayerGameObject = cutSceneBackup.VideoPlayerGameObject;

        Step.OnStepInitialized += HandleOnStepInitialized;
        DialogBox.OnNextButtonPressed += HandleOnNextStepButtonPressed;
        DialogBox.OnSkipButtonPressed += HandleOnCutSceneSkip;

        NextStepAnimationEvent.OnNextStepAnimationEvent += ShowNextStep;

        PlayCutScene();
    }

    public void PlayCutScene()
    {
        if (CurrentCutscene != null)
        {
            DialogTrigger.SetText(CurrentCutscene);
            DialogTrigger.CreateDialogBox();

            //DialogBox.SetTextSO(CurrentCutscene);
            //DialogBox.PlayStep(0);
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
    }

    private void HandleOnNextStepButtonPressed()
    {
        if (!DialogBox.ShowTextInProgress)
        {
            // The Fade-In-Out animation triggers the ShowNextStep() by default.
            if (DialogBox.StepCounter < CurrentCutscene.Steps.Count - 1)
            {
                DialogBox.SetCanNextStep(false);
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
        DialogBox.OnSkipButtonPressed -= HandleOnCutSceneSkip;
        DialogBox.OnNextButtonPressed -= HandleOnNextStepButtonPressed;
        Step.OnStepInitialized -= HandleOnStepInitialized;

        NextStepAnimationEvent.OnNextStepAnimationEvent -= ShowNextStep;

        OnCutSceneSkipRequest?.Invoke();
    }

    public void SetCutScene(CutSceneSO cutscene)
    {
        _currentCutscene = cutscene;
    }
}
