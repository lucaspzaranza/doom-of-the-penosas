using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public enum Penosas
{
    None = -1,
    Geruza = 0,
    Dolores = 1
}

public class PlayerSelectionInputController : MonoBehaviour
{
    public static PlayerSelectionInputController instance;
    [SerializeField] private PlayerSelectionData[] playersSelectionData;
    [SerializeField] private float arrowXOffset;

    private PlayerInput playerInputActions;
    private InputAction navigation;
    private InputAction selectPlayer;
    private InputAction cancelOrBack;
    private bool playersAreSelected = false;
    private int current1PPlayerIndex = 0;

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

    private void SetArrowPosition(int arrowIndex, Vector2 newPos)
    {
        playersSelectionData[arrowIndex].Arrow.transform.localPosition = newPos;
    }

    private void ChangePlayerSelection(CallbackContext context)
    {
        if (playersAreSelected) return;

        float direction = navigation.ReadValue<float>();
        if (direction < 0) // Left
        {
            if (PlayerCount == 1)
            {
                SetArrowPosition(0, new Vector2(playersSelectionData[0].ArrowXInitCoord, transform.localPosition.y));
                current1PPlayerIndex = 0; // Index do jogador 1 indo pra segunda penosa.
            }
        }
        else if (direction > 0) // Right
        {
            if (PlayerCount == 1)
            {
                SetArrowPosition(0, new Vector2(playersSelectionData[1].ArrowXInitCoord, transform.localPosition.y));
                current1PPlayerIndex = 1; // Index do jogador 1 indo pra primeira penosa.
            }
        }
    }

    private void Cancel(CallbackContext context)
    {
        if(playersAreSelected)
        {
            playersAreSelected = false;
            SetArrowPosition(0, new Vector2(playersSelectionData[current1PPlayerIndex].ArrowXInitCoord, transform.localPosition.y));
        }
        else
        {
            print("Voltar ao menu principal.");
        }
    }

    private void Select(CallbackContext context)
    {
        if (!playersAreSelected)
        {
            if (PlayerCount == 1)
            {
                playersSelectionData[0].SelectedPenosa = (Penosas)current1PPlayerIndex;

                // Forma de pegar o índice complementar. Se f(0) = (0 + 1) % 2 = 1. Se f(1) = (1 + 1) % 2 = 0;
                PlayerSelectionUIController.instance.PenosasTexts[(current1PPlayerIndex + 1) % 2].color = Color.white;
                PlayerSelectionUIController.instance.PenosasTexts[current1PPlayerIndex].color = Color.red;
                PlayerSelectionUIController.instance.StartText.SetActive(true);
                Vector2 startTxtPos = PlayerSelectionUIController.instance.StartText.transform.localPosition;
                SetArrowPosition(0, startTxtPos);
                playersAreSelected = true;
            }
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("SampleScene");
        }
    }
}
