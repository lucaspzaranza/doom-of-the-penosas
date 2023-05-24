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
            ConstantStrings.ControllerNotFoundOnBackupMessage(nameof(MapaMundiController));
            return;
        }

        _backToMainMenuBtn.onClick.AddListener(() =>
        {
            var sceneController = FindAnyObjectByType<SceneController>();
            if (sceneController != null)
                sceneController.LoadScene(ScenesBuildIndexes.MainMenu);
            else
                SceneManager.LoadScene(ScenesBuildIndexes.MainMenu);
                mapaMundiController.gameObject.SetActive(false);
        });

        // Man, I have to find a way to do this with some loop instead using these hard coded values...
        _stageButtons[0].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(0);
        });

        _stageButtons[1].onClick.AddListener(() =>
        {
            mapaMundiController.SelectSceneIndex(1);
        });
    }
}
