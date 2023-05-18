using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;

public class GameController : Controller
{
    [Header("Game General Data")]
    [SerializeField] private GameMode _gameMode;
    public GameMode GameMode => _gameMode;

    [SerializeField] private GameStatus _gameStatus;
    public GameStatus GameStatus => _gameStatus;

    [Header("Controllers")]

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;
    [SerializeField] private UIController _uiController;
    public UIController UIController => _uiController;

    [SerializeField] private PoolController _poolController;
    public PoolController PoolController => _poolController;

    void Start()
    {
        Setup();

        DontDestroyOnLoad(gameObject);
    }

    public override void Setup()
    {
        UIController?.Setup();

        EventHandlerSetup();       
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;

        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

        UIController.OnOnGameModeSelected += SetGameMode;
        UIController.OnGameStart -= HandleUIControllerOnGameStart;

        PlayerController.Dispose();
        UIController.Dispose();
        PoolController.Dispose();
    }

    private void EventHandlerSetup()
    {
        //Penosa.OnPlayerDeath += PlayerController.RemovePlayerFromScene;

        //PlayerController.OnGameOverCountdownTextIsNull += HandleOnGameOverCountdownTextIsNull;

        //PlayerController.OnCountdownActivation += HandleOnCoutdownActivation;

        UIController.OnOnGameModeSelected += SetGameMode;
        UIController.OnGameStart += HandleUIControllerOnGameStart;
    }

    public void StartGame()
    {
        _gameStatus = GameStatus.Loading;

        PlayerController.Setup();
        PoolController.Setup();
        UIController.InGameSetup();
    }    

    public void GetAllControllers()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    private void HandleOnGameOverCountdownTextIsNull()
    {
        Text countdown = UIController.GetCountdownTextFromInGameController();
        PlayerController.SetGameOverCountdownText(countdown);
    }

    private void HandleOnCoutdownActivation(bool val)
    {
        UIController.GameOverActivation(val);
    }

    private void SetGameMode(GameMode gameMode)
    {
        _gameMode = gameMode;
    }

    private void HandleUIControllerOnGameStart(IReadOnlyList<Penosas> characterSelectionList)
    {
        foreach (var character in characterSelectionList)
        {
            print(character.ToString());
        }
    }
}