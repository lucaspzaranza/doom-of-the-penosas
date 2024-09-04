using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutSceneControllerBackup : ControllerBackup
{
    [Header("Scene Buttons")]
    [SerializeField] private Button _nextStepBtn;
    public Button NextStepBtn => _nextStepBtn;

    [SerializeField] private Button _skipBtn;
    public Button SkipBtn => _skipBtn;

    [SerializeField] private TextMeshProUGUI _cutSceneTMPro;
    public TextMeshProUGUI CutSceneTMPro => _cutSceneTMPro;

    [SerializeField] private VideoPlayer _videoPlayer;
    public VideoPlayer VideoPlayer => _videoPlayer;

    [SerializeField] private Image _image;
    public Image Image => _image;

    protected override Type GetControllerType() => typeof(CutSceneController);

    protected override void ListenersSetup()
    {
        var cutSceneController = _controller as CutSceneController;

        if (cutSceneController == null)
        {
            WarningMessages.ControllerNotFoundOnBackupMessage(nameof(CutSceneController));
            return;
        }

        _nextStepBtn.onClick.AddListener(() =>
        {
            cutSceneController.CurrentCutscene.NextStepButtonPressed();
        });

        _skipBtn.interactable = cutSceneController.CurrentCutscene.Skippable;

        _skipBtn.onClick.AddListener(() =>
        {
            cutSceneController.CurrentCutscene.Skip();
        });
    }
}
