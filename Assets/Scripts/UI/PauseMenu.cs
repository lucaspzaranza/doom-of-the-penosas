using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SharedData.Enumerations;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _backToMainMenuBtn;
    [SerializeField] private GameController _gameController;

    private int _frameCounter = 0;

    private void OnEnable()
    {
        _gameController = FindAnyObjectByType<GameController>();

        if (_gameController == null)
            return;

        _frameCounter = 0;
    }

    private void Update()
    {
        // The only way I found to assign these listeners without the PauseMenu button
        // be called when the user press it for the first time and deactivate it immediately
        if (_frameCounter == 1)
            ButtonsListenersSetup();

        if(_frameCounter <= 1)
            _frameCounter++;   
    }

    private void ButtonsListenersSetup()
    {
        _resumeBtn.onClick.AddListener(() =>
        {
            if(_gameController.GameIsPaused)
                _gameController.PlayerController.OnPlayerPause?.Invoke(false);
        });

        _backToMainMenuBtn.onClick.AddListener(() =>
        {
            _gameController.PlayerController.OnPlayerPause?.Invoke(false);
            _gameController.SetGameStatus(GameStatus.Menu);
            _gameController.SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);
        });
    }

    private void OnDisable()
    {
        _resumeBtn.onClick.RemoveAllListeners();
        _backToMainMenuBtn.onClick.RemoveAllListeners();
    }
}
