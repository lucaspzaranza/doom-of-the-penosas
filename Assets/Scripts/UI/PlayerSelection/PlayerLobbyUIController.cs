using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class PlayerLobbyUIController : ControllerUnit, IUIController
{
    public Action<GameMode> OnGameModeButtonPressed;
    public Action<GameObject> OnGameReadyToStart;
    public Action<IReadOnlyList<Penosas>> OnGameStart;
    public Action OnCancelSelection;

    // Vars
    [Space]
    [SerializeField] private LobbyState _lobbyState;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMainMenuButton;
    [SerializeField] private GameObject _cancelCharacterSelectionButton; 
    [SerializeField] private GameObject _mainMenu;

    [Header("Chosen Characters")]
    [SerializeField] private List<Penosas> _characterSelectionList;
    public List<Penosas> CharacterSelectionList => _characterSelectionList;

    [Space]
    [SerializeField] private List<Button> _characterButtons;
    [SerializeField] private List<TMP_Text> _charactersTexts;
    [SerializeField] private Color _defaultTextColor;
    [SerializeField] private List<Color> _selectedColors;

    private Button _startBtnComponent;
    private Button _cancelBtnComponent;

    private void OnEnable()
    {
        _lobbyState = LobbyState.GameModeSelection;
    }

    /// <summary>
    /// Se passar o índice 0, retorna o índice 1, e vice-versa.
    /// </summary>
    private int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }
    
    public void SelectButton(GameObject buttonToSelect)
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void Setup()
    {
        _parentController = GetComponentInParent<UIController>();

        _startBtnComponent = _startButton.GetComponent<Button>();
        _cancelBtnComponent = _backToMainMenuButton.GetComponent<Button>();
    }

    //public override void Dispose()
    //{

    //}

    public override GameMode GetGameMode()
    {
        return ((ControllerUnit)_parentController).GetGameMode();
    }

    public void SetGameMode(int gameMode)
    {
        OnGameModeButtonPressed?.Invoke((GameMode)gameMode);
    }

    public void SetLobbyState(int lobbyState)
    {
        _lobbyState = (LobbyState)lobbyState;
    }

    public void ChooseCharacter(int characterIndex)
    {
        if (_lobbyState != LobbyState.PlayerSelection)
            return;

        if (_characterSelectionList == null)
            ResetSelection();

        _characterSelectionList.Add((Penosas)characterIndex);
        SetCharacterTextColors(characterIndex, _selectedColors[0]);

        // Considering a 2-player multiplayer logic
        if (GetGameMode() == GameMode.Multiplayer)
        {
            int otherPlayer = GetComplementaryPlayerIndex(characterIndex);
            _characterSelectionList.Add((Penosas)otherPlayer);
            SetCharacterTextColors(otherPlayer, _selectedColors[1]);
        }

        _lobbyState = LobbyState.ReadyToStart;
        SetGameActivation(true);
    }

    private void SetCharacterTextColors(int characterIndex, Color color)
    {
        _charactersTexts[characterIndex].color = color;
    }

    public void SetGameActivation(bool val)
    {
        _cancelCharacterSelectionButton.SetActive(val);
        _backToMainMenuButton.SetActive(!val);

        var startBtn = _startButton.GetComponent<Button>();
        startBtn.interactable = val;

        _characterButtons.ForEach(btn =>
        {
            var btnComponent = btn.interactable = !val;
        });

        if (val)
        {
            // _startButton is the button the cursor must go when the game is about to start
            OnGameReadyToStart?.Invoke(_startButton);
        }
        else
            OnCancelSelection?.Invoke();
    }
    
    public void ResetSelection()
    {
        _characterSelectionList = new List<Penosas>();
    }

    public void CancelCharacterSelection()
    {
        if (_lobbyState != LobbyState.ReadyToStart)
            return;

        ResetSelection();

        _lobbyState = LobbyState.PlayerSelection;
        _charactersTexts.ForEach(_text =>
        {
            SetCharacterTextColors(_charactersTexts.IndexOf(_text), _defaultTextColor);
        });
        SetGameActivation(false);
    }

    public void FireStartGame()
    {
        OnGameStart?.Invoke(CharacterSelectionList);
    }
}