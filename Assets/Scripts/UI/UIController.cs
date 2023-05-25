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
    public Action<int> OnUIGameSelectedSceneIndex;
    public Action OnUIBackToMainMenuFromMapaMundi;

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

    public Text GetCountdownTextFromInGameController()
    {
        return PlayerInGameUIController.GetCountdownText();
    }

    public void GameOverActivation(bool val)
    {
        PlayerInGameUIController.GameOverContainerActivation(val);
    }

    private void ReturnCursorsToDefaultPosition()
    {
        _cursors.ForEach(cursor =>
        {
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

            _mapaMundiController.OnGameSceneIndexSelected += HandleMapaMundiOnSceneIndexSelected;
            _mapaMundiController.OnBackToMainMenu += HandleOnMapaMundiBackToMainMenu;
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
            _playerInGameUIController.Setup();
        }
        else
        {
            if (!_playerInGameUIController.gameObject.activeSelf)
                _playerInGameUIController.gameObject.SetActive(true);
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
}
