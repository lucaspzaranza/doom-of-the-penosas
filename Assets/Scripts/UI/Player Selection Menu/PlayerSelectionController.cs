using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using SharedData.Enumerations;


public class PlayerSelectionController : MonoBehaviour
{
    public static PlayerSelectionController instance;
    [SerializeField] private PlayerSelectionData[] playersSelectionData;
    [SerializeField] private float arrowXOffset;


    private PlayerInput playerInputActions;
    private InputAction navigation;
    private InputAction selectPlayer;
    private InputAction cancelOrBack;
    private int current1PPlayerIndex = 0;

    // Alias pra encurtar o comprimento das linhas.
    private PlayerSelectionMenuState MenuState => PlayerSelectionUIController.instance.MenuState;
    public int PlayerCount { get; set; } = 1;
    public PlayerSelectionData[] PlayersSelectionData => playersSelectionData;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.performed += ChangePlayerSelection;
        navigation.Enable();

        selectPlayer = playerInputActions.PlayerSelectionMenu.SelectPlayer;
        selectPlayer.performed += Select;
        selectPlayer.Enable();

        cancelOrBack = playerInputActions.PlayerSelectionMenu.CancelorBack;
        cancelOrBack.performed += Cancel;
        cancelOrBack.Enable();

        playersSelectionData[0].SetArrowXInitCoordValue(playersSelectionData[0].Arrow.transform.localPosition.x);
        playersSelectionData[1].SetArrowXInitCoordValue(playersSelectionData[1].Arrow.transform.localPosition.x);
    }

    private void OnDisable()
    {
        navigation.performed -= ChangePlayerSelection;
        navigation.Disable();

        selectPlayer.performed -= Select;
        selectPlayer.Disable();

        cancelOrBack.performed -= Cancel;
        cancelOrBack.Disable();
    }

    private void SetArrowPosition(Vector2 newPos)
    {
        PlayersSelectionData[0].Arrow.transform.localPosition = newPos;
    }

    private void ChangePlayerSelection(CallbackContext context)
    {
        if (MenuState == PlayerSelectionMenuState.ReadyToStart) return;

        float direction = navigation.ReadValue<float>();
        if (direction < 0) // Left
        {
            if (PlayerCount == 1)
            {
                SetArrowPosition(new Vector2(PlayersSelectionData[0].ArrowXInitCoord, transform.localPosition.y));
                current1PPlayerIndex = 0; // Index do jogador 1 indo pra segunda penosa.
            }
        }
        else if (direction > 0) // Right
        {
            if (PlayerCount == 1)
            {
                SetArrowPosition(new Vector2(PlayersSelectionData[1].ArrowXInitCoord, transform.localPosition.y));
                current1PPlayerIndex = 1; // Index do jogador 1 indo pra primeira penosa.
            }
        }
    }

    private void Cancel(CallbackContext context)
    {
        if(MenuState == PlayerSelectionMenuState.ReadyToStart)
        {
            SetPlayerNameTextColor(current1PPlayerIndex, Color.white);
            SetArrowPosition(new Vector2(PlayersSelectionData[current1PPlayerIndex].ArrowXInitCoord, transform.localPosition.y));
            PlayerSelectionUIController.instance.SetState(PlayerSelectionMenuState.PlayerSelection);
        }
        else if(MenuState == PlayerSelectionMenuState.PlayerSelection)
            print("Voltar ao menu principal.");
    }

    private void SetPlayerNameTextColor(int index, Color newColor)
    {
        PlayerSelectionUIController.instance.PenosasTexts[index].color = newColor;
    }

    private void Select(CallbackContext context)
    {
        //if (!playersAreSelected)
        if (MenuState == PlayerSelectionMenuState.PlayerSelection)
        {
            if (PlayerCount == 1)
            {
                PlayersSelectionData[0].SelectedPenosa = (Penosas)current1PPlayerIndex;

                // Forma de pegar o índice complementar. Se f(0) = (0 + 1) % 2 = 1. Se f(1) = (1 + 1) % 2 = 0;
                SetPlayerNameTextColor((current1PPlayerIndex + 1) % 2, Color.white);
                SetPlayerNameTextColor(current1PPlayerIndex, Color.red);

                PlayerSelectionUIController.instance.StartText.SetActive(true);
                Vector2 startTxtPos = PlayerSelectionUIController.instance.StartText.transform.localPosition;
                SetArrowPosition(startTxtPos);
                PlayerSelectionUIController.instance.SetState(PlayerSelectionMenuState.ReadyToStart);
            }
        }
        else if(MenuState == PlayerSelectionMenuState.ReadyToStart)
        {
            var newPenosa = Instantiate(PlayersSelectionData[current1PPlayerIndex].Prefab, transform.position, Quaternion.identity);
            DontDestroyOnLoad(newPenosa);
            SceneManager.LoadScene("SampleScene");
        }
    }
}
