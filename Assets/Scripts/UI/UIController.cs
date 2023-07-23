using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : ControllerUnit
{
    public Action<GameMode> OnUIGameModeSelected;
    public Action<bool> OnUISetNewGame;
    public Action<IReadOnlyList<Penosas>> OnUISelectedCharacters;
    public Action<IReadOnlyList<InputDevice>> OnUISelectedDevices;
    public Action<int> OnUIGameSelectedSceneIndex;
    public Action OnUIBackToMainMenuFromMapaMundi;
    public Action<byte> OnCountdownIsOver;

    [Space]
    [Header("Variables")]
    [SerializeField] private Canvas _gameSceneCanvas;

    [Space]
    [Header("UI Controllers")]
    [SerializeField] private PlayerLobbyUIController _playerLobbyUIController;
    public PlayerLobbyUIController PlayerLobbyUIController => _playerLobbyUIController;

    [SerializeField] private PlayerInGameUIController _playerInGameUIController;
    public PlayerInGameUIController PlayerInGameUIController => _playerInGameUIController;

    [SerializeField] private MapaMundiController _mapaMundiController;
    public MapaMundiController MapaMundiController => _mapaMundiController;

    [Header("Cursor")]
    [SerializeField] private List<CursorPosition> _cursors;
    public List<CursorPosition> CursorPositions => _cursors;

    [Header("Prefabs")]
    [SerializeField] private GameObject _pauseMenuPrefab;
    private GameObject _pauseMenuInstance;

    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverContainer;

    public override void Setup()
    {
        base.Setup();

        _playerLobbyUIController = FindAnyObjectByType<PlayerLobbyUIController>();

        if (PlayerLobbyUIController != null)
        {
            if(!PlayerLobbyUIController.gameObject.activeSelf)
                PlayerLobbyUIController.gameObject.SetActive(true);

            PlayerLobbyUIController.Setup();
            PlayerLobbyUIController.OnGameModeButtonPressed += HandleLobbyOnGameModeButtonPressed;
            PlayerLobbyUIController.OnGameReadyToStart += HandleLobbyOnGameReadyToStart;
            PlayerLobbyUIController.OnCancelSelection += OnLobbyCancelSelection;
            PlayerLobbyUIController.OnLobbySelectedCharacters += HandleLobbyOnGameSelectedCharacters;
            PlayerLobbyUIController.OnLobbySetNewGame += HandleOnLobbySetNewGame;
            PlayerLobbyUIController.OnLobbySelectedDevices += HandleOnLobbySelectedDevices;
        }

        MenuWithCursor.OnMenuEnabled += HandleMenuWithCursorEnabled;
    }

    public override void Dispose()
    {
        PlayerLobbyUIController.OnGameModeButtonPressed -= HandleLobbyOnGameModeButtonPressed;
        PlayerLobbyUIController.OnGameReadyToStart -= HandleLobbyOnGameReadyToStart;
        PlayerLobbyUIController.OnCancelSelection -= OnLobbyCancelSelection;
        PlayerLobbyUIController.OnLobbySelectedCharacters -= HandleLobbyOnGameSelectedCharacters;
        PlayerLobbyUIController.OnLobbySetNewGame -= HandleOnLobbySetNewGame;
        PlayerLobbyUIController.OnLobbySelectedDevices -= HandleOnLobbySelectedDevices;
        PlayerLobbyUIController.Dispose();

        MenuWithCursor.OnMenuEnabled -= HandleMenuWithCursorEnabled;
    }

    private void HandleOnLobbySetNewGame(bool value)
    {
        OnUISetNewGame?.Invoke(value);
    }

    private void HandleLobbyOnGameModeButtonPressed(GameMode gameMode)
    {
        OnUIGameModeSelected?.Invoke(gameMode);
    }

    public void CountdownActivation(byte playerID, bool val)
    {
        PlayerInGameUIController.ContinueActivation(playerID, val);
    }

    private void ReturnCursorsToDefaultPosition()
    {
        _cursors.ForEach(cursor =>
        {
            if(cursor != null)
                SetCursorPosition(cursor, cursor.DefaultCursorParent);
        });
    }

    private void ReturnCursorsToLastPosition()
    {
        _cursors.ForEach(cursor =>
        {
            SetCursorPosition(cursor, cursor.LastSelected);
        });
    }

    private void ReturnCursorsToLastPressedButton()
    {
        _cursors.ForEach(cursor =>
        {
            if(cursor.LastPressed != null) 
                SetCursorPosition(cursor, cursor.LastPressed);
        });
    }

    private void HandleMenuWithCursorEnabled(IReadOnlyList<CursorPosition> newCursors)
    {
        _cursors = new List<CursorPosition>(newCursors);
        ReturnCursorsToDefaultPosition();
    }

    private void HandleLobbyOnGameReadyToStart(GameObject target)
    {
        SetCursorPosition(CursorPositions[0], target);
    }

    private void OnLobbyCancelSelection()
    {
        ReturnCursorsToLastPressedButton();
    }

    public void SetCursorPosition(CursorPosition cursor, GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
        cursor.UpdateCursorPosition(target);
    }

    private void HandleLobbyOnGameSelectedCharacters(IReadOnlyList<Penosas> selectedCharacters)
    {
        OnUISelectedCharacters?.Invoke(selectedCharacters);
    }
   
    public void InGameUIControllerSetup()
    {
        _playerInGameUIController.Setup();
    }

    public void DisposeLobbyController()
    {
        PlayerLobbyUIController.Dispose();
    }

    public void InstantiateMapaMundiController()
    {
        if (_mapaMundiController == null)
        {
            var prefab = GetControllerFromPrefabList<MapaMundiController>();
            var instance = Instantiate(prefab, transform);
            _mapaMundiController = instance.GetComponent<MapaMundiController>();
            _mapaMundiController.Setup();

            GameController gameCtrl = _parentController as GameController;

            _mapaMundiController.OnGameSceneIndexSelected += HandleMapaMundiOnSceneIndexSelected;
            _mapaMundiController.OnBackToMainMenu += HandleOnMapaMundiBackToMainMenu;
            _mapaMundiController.OnReferencesLoaded += gameCtrl.HandleOnMapaMundiReferencesLoaded;
        }
        else
        {
            if (!_mapaMundiController.gameObject.activeSelf)
                _mapaMundiController.gameObject.SetActive(true);
        }
    }

    public void InstantiatePlayerInGameUIController()
    {
        if (_playerInGameUIController == null)
        {
            var prefab = GetControllerFromPrefabList<PlayerInGameUIController>();
            var instance = Instantiate(prefab, transform);
            _playerInGameUIController = instance.GetComponent<PlayerInGameUIController>();
            _playerInGameUIController.Setup(_gameSceneCanvas, 
                TryToGetGameControllerFromParent().PlayerController.PlayersData);
        }
        else
        {
            if (!_playerInGameUIController.gameObject.activeSelf)
                _playerInGameUIController.gameObject.SetActive(true);

            _playerInGameUIController.Setup(_gameSceneCanvas,
                TryToGetGameControllerFromParent().PlayerController.PlayersData);
        }
    }

    private void HandleMapaMundiOnSceneIndexSelected(int buildIndex)
    {
        OnUIGameSelectedSceneIndex?.Invoke(buildIndex);
    }

    private void HandleOnMapaMundiBackToMainMenu()
    {
        MapaMundiController.Dispose();

        OnUIBackToMainMenuFromMapaMundi?.Invoke();
    }

    private void HandleOnLobbySelectedDevices(IReadOnlyList<InputDevice> devices)
    {
        OnUISelectedDevices?.Invoke(devices);
    }

    public void SelectGameSceneCanvas()
    {
        _gameSceneCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .FirstOrDefault(canvas => canvas.transform.parent == null);
    }

    public void PauseMenuActivation(bool val)
    {
        if(_pauseMenuInstance == null && val)
        {
            _pauseMenuInstance = Instantiate(_pauseMenuPrefab, _gameSceneCanvas.transform);
            return;
        }

        _pauseMenuInstance.SetActive(val);
    }

    public void GameOverActivation()
    {
        _playerInGameUIController.HideHUDs();
        Instantiate(_gameOverContainer, _gameSceneCanvas.transform);
    }
}
