using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;

public class GameController : MonoBehaviour, IController
{
    public static GameController instance;

    [SerializeField] private GameStatus _gameStatus;
    public GameStatus GameStatus => _gameStatus;

    [Header("Controllers")]

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;
    [SerializeField] private UIController _uiController;
    public UIController UIController => _uiController;

    [SerializeField] private PoolController _poolController;
    public PoolController PoolController => _poolController;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        //InitiateProjectilesPools();
        Setup();

        DontDestroyOnLoad(gameObject);
    }

    public void Setup()
    {
        EventHandlerSetup();       
    }

    private void EventHandlerSetup()
    {
        Penosa.OnPlayerDeath += PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull += HandleOnGameOverCountdownTextIsNull;

        PlayerController.OnCountdownActivation += HandleOnCoutdownActivation;
    }

    public void Dispose() 
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;

        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

        PlayerController.Dispose();
        UIController.Dispose();
        PoolController.Dispose();
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
}