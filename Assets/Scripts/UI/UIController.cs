using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UIController : ControllerUnit
{
    public Action<GameMode> OnOnGameModeSelected;
    public Action<IReadOnlyList<Penosas>> OnGameStart;

    [Space]
    [Header("UI Controllers")]
    [SerializeField] private PlayerLobbyUIController _playerSelectionUIController;
    [SerializeField] private PlayerInGameUIController _playerInGameUIController;

    [Header("Cursor")]
    [SerializeField] private List<CursorPosition> _cursors;
    public List<CursorPosition> CursorPositions => _cursors;

    [Header("Prefabs")]
    [SerializeField] private GameObject _selectionController;
    [SerializeField] private GameObject _inGameController;

    // Props
    public PlayerLobbyUIController PlayerLobbyUIController => _playerSelectionUIController;
    public PlayerInGameUIController PlayerInGameUIController => _playerInGameUIController;

    public override void Setup()
    {
        base.Setup();

        if(PlayerLobbyUIController != null)
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
        OnOnGameModeSelected?.Invoke(gameMode);
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
            SetCursorPosition(cursor, cursor.LastPressed);
        });
    }

    private void HandleMenuWithCursorEnabled(List<CursorPosition> newCursors)
    {
        _cursors = new List<CursorPosition>(newCursors);
        ReturnCursorsToDefaultPosition();
    }

    private void HandleLobbyOnGameReadyToStart(GameObject target)
    {
        if(GetGameMode() == GameMode.Singleplayer)
            SetCursorPosition(CursorPositions[0], target);
        else
        {
            CursorPositions.ForEach(cursor =>
            {
                SetCursorPosition(cursor, target);
            });
        }
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
}
