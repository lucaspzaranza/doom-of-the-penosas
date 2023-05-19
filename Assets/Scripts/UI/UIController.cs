using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UIController : ControllerUnit
{
    public static Action<GameMode> OnGameModeSelected;
    public Action<IReadOnlyList<Penosas>> OnGameStart;

    [Space]
    [Header("UI Controllers")]
    [SerializeField] private PlayerLobbyUIController _playerLobbyUIController;
    [SerializeField] private PlayerInGameUIController _playerInGameUIController;

    [Header("Cursor")]
    [SerializeField] private List<CursorPosition> _cursors;
    public List<CursorPosition> CursorPositions => _cursors;

    // Props
    public PlayerLobbyUIController PlayerLobbyUIController => _playerLobbyUIController;
    public PlayerInGameUIController PlayerInGameUIController => _playerInGameUIController;

    public override void Setup()
    {
        base.Setup();

        _playerLobbyUIController = FindAnyObjectByType<PlayerLobbyUIController>();

        if (PlayerLobbyUIController != null)
        {
            PlayerLobbyUIController.Setup();
            PlayerLobbyUIController.OnGameModeButtonPressed += HandleLobbyOnGameModeButtonPressed;
            PlayerLobbyUIController.OnGameReadyToStart += HandleLobbyOnGameReadyToStart;
            PlayerLobbyUIController.OnCancelSelection += OnLobbyCancelSelection;
            PlayerLobbyUIController.OnGameStart += HandleLobbyOnGameStart;
        }

        MenuWithCursor.OnMenuEnabled += HandleMenuWithCursorEnabled;
    }

    public override void Dispose()
    {
        PlayerLobbyUIController.OnGameModeButtonPressed -= HandleLobbyOnGameModeButtonPressed;
        PlayerLobbyUIController.OnGameReadyToStart -= HandleLobbyOnGameReadyToStart;
        PlayerLobbyUIController.OnCancelSelection -= OnLobbyCancelSelection;
        PlayerLobbyUIController.OnGameStart -= HandleLobbyOnGameStart;
        PlayerLobbyUIController.Dispose();

        MenuWithCursor.OnMenuEnabled -= HandleMenuWithCursorEnabled;
    }

    private void HandleLobbyOnGameModeButtonPressed(GameMode gameMode)
    {
        OnGameModeSelected?.Invoke(gameMode);
    }

    public Text GetCountdownTextFromInGameController()
    {
        return PlayerInGameUIController.GetCountdownText();
    }

    public void GameOverActivation(bool val)
    {
        PlayerInGameUIController.GameOverContainerActivation(val);
    }

    public void InGameSetup()
    {
        PlayerInGameUIController.Setup();
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

    private void HandleLobbyOnGameStart(IReadOnlyList<Penosas> selectedCharacters)
    {
        OnGameStart?.Invoke(selectedCharacters);
    }

    public void SetInGameUIController(PlayerInGameUIController inGameUIController)
    {
        _playerInGameUIController = inGameUIController;
    }

    public void InGameUIControllerSetup()
    {
        _playerInGameUIController.Setup();
    }

    public void DisposeLobbyController()
    {
        var lobbyController = ChildControllersPrefabs.FirstOrDefault(ctrl => ctrl.GetComponent<PlayerLobbyUIController>());
        ChildControllersPrefabs.Remove(lobbyController);

        var controllerComponent = lobbyController.GetComponent<PlayerLobbyUIController>();
        //DestroyImmediate(lobbyController, true);
    }
}
