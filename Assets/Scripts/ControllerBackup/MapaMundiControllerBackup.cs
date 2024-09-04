using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapaMundiControllerBackup : ControllerBackup
{
    [Header("Scene Buttons")]
    [SerializeField] private List<Button> _stageButtons;
    public List<Button> StageButtons => _stageButtons;

    [SerializeField] private Button _backToMainMenuBtn;
    public Button BackToMainMenuButton => _backToMainMenuBtn;

    protected override Type GetControllerType() => typeof(MapaMundiController);

    protected override void ListenersSetup()
    {
        var mapaMundiController = _controller as MapaMundiController;

        if (mapaMundiController == null)
        {
            WarningMessages.ControllerNotFoundOnBackupMessage(nameof(MapaMundiController));
            return;
        }

        _backToMainMenuBtn.onClick.AddListener(() =>
        {
            mapaMundiController.FireBackToMainMenuEvent();
        });

        _stageButtons[0].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._1stStage);
        });

        _stageButtons[1].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._2ndStage);
        });

        _stageButtons[2].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._3rdStage);
        });

        _stageButtons[3].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._4thStage);
        });

        _stageButtons[4].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._5thStage);
        });

        _stageButtons[5].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(ScenesBuildIndexes._6thStage);
        });
    }
}
