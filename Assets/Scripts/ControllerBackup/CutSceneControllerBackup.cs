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

    [SerializeField] private TextMeshProUGUI _cutSceneTMPro;
    public TextMeshProUGUI CutSceneTMPro => _cutSceneTMPro;

    [SerializeField] private DialogBox _dialogBox;
    public DialogBox DialogBox => _dialogBox;

    [SerializeField] private VideoPlayer _videoPlayer;
    public VideoPlayer VideoPlayer => _videoPlayer;

    [SerializeField] private GameObject _videoPlayerGameObject;
    public GameObject VideoPlayerGameObject => _videoPlayerGameObject;

    [SerializeField] private Image _image;
    public Image Image => _image;

    [SerializeField] private GameObject _fadeInOut;
    public GameObject FadeInOut => _fadeInOut;

    [SerializeField] private DialogTrigger _dialogTrigger;
    public DialogTrigger DialogTrigger => _dialogTrigger;

    protected override Type GetControllerType() => typeof(CutSceneController);

    protected override void ListenersSetup()
    {
        var cutSceneController = _controller as CutSceneController;

        if (cutSceneController == null)
        {
            WarningMessages.ControllerNotFoundOnBackupMessage(nameof(CutSceneController));
            return;
        }
    }
}
