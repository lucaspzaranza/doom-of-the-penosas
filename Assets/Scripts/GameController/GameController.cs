using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;
using UnityEngine.SceneManagement;
using System;

public class GameController : Controller
{
    // Singleton instance
    private static GameController instance;

    // Props
    [SerializeField] private GameMode _gameMode;
    public GameMode GameMode => _gameMode;

    [SerializeField] private GameStatus _gameStatus;
    public GameStatus GameStatus => _gameStatus;

    [SerializeField] private bool _isNewGame;
    public bool IsNewGame => _isNewGame;

    private List<Penosas> _characterSelectionList;
    public IReadOnlyList<Penosas> CharacterSelectionList => _characterSelectionList;

    [Header("Controllers")]

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;

    [SerializeField] private UIController _uiController;
    public UIController UIController => _uiController;

    [SerializeField] private PoolController _poolController;
    public PoolController PoolController => _poolController;

    [SerializeField] private SceneController _sceneController;
    public SceneController SceneController => _sceneController;   

    private bool IsSingleInstance => instance == this;

    // Vars
    [Header("Game General Data")]
    [SerializeField] private GameMode _gameModeState; // Variable to reflect the Controller Game Mode only in the Editor.     

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if(IsSingleInstance)
        {
            Setup();
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void Setup()
    {
        UIController.Setup();
        SceneController.Setup();

        EventHandlerSetup();       
    }

    private void EventHandlerSetup()
    {
        UIController.OnUISelectedCharacters += HandleOnUISelectedCharacters;
        UIController.OnUIGameModeSelected += SetGameMode;
        UIController.OnUISetNewGame += SetNewGame;
        UIController.OnUIGameSelectedSceneIndex += HandleOnGameSceneSelectedIndex;
        UIController.OnUIBackToMainMenuFromMapaMundi += HandleOnUIBackToMainMenuFromMapaMundi;

        SceneController.OnSceneLoaded += HandleOnSceneLoaded;
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;
        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

        UIController.OnUISelectedCharacters -= HandleOnUISelectedCharacters;
        UIController.OnUIGameModeSelected -= SetGameMode;
        UIController.OnUISetNewGame -= SetNewGame;
        UIController.OnUIGameSelectedSceneIndex -= HandleOnGameSceneSelectedIndex;
        UIController.OnUIBackToMainMenuFromMapaMundi -= HandleOnUIBackToMainMenuFromMapaMundi;

        SceneController.OnSceneLoaded -= HandleOnSceneLoaded;

        PlayerController.Dispose();
        UIController.Dispose();
        PoolController.Dispose();
    }

    private void SetGameMode(GameMode newGameMode)
    {
        _gameMode = newGameMode;
    }

    private void SetNewGame(bool val)
    {
        _isNewGame = val;
    }

    public void SetGameStatus(GameStatus gameStatus)
    {
        _gameStatus = gameStatus;
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
   
    private void HandleOnUISelectedCharacters(IReadOnlyList<Penosas> characterSelectionList)
    {
        UIController.DisposeLobbyController();

        if(_characterSelectionList == null)
            _characterSelectionList = new List<Penosas>();
        _characterSelectionList = characterSelectionList.ToList();

        SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);

        UIController.InstantiateMapaMundiController();
    }
   
    // Change it
    private void InGameControllersSetup()
    {
        _playerController.Setup();
        _poolController.Setup();
        UIController.InGameUIControllerSetup();

        Penosa.OnPlayerDeath += PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull += HandleOnGameOverCountdownTextIsNull;
        PlayerController.OnCountdownActivation += HandleOnCoutdownActivation;
    }    

    private void HandleOnSceneLoaded(Scene scene)
    {
        if(scene.buildIndex == ScenesBuildIndexes.MainMenu)
        {
            if(GameStatus == GameStatus.Loading)
            {
                SetGameStatus(GameStatus.InGame);
                InGameControllersSetup();
            }
            else if(GameStatus == GameStatus.Menu)
                UIController.PlayerLobbyUIController.gameObject.SetActive(true);
        }
        else if(scene.buildIndex == ScenesBuildIndexes._1stStage)
        {
            if (GameStatus == GameStatus.Loading)
            {
                SetGameStatus(GameStatus.InGame);
                PlayerController.AddPlayers();
            }
        }
    }

    private void InstantiatePlayerController()
    {
        var playerControllerPrefab = GetControllerFromPrefabList<PlayerController>();
        if (playerControllerPrefab != null && _playerController == null)
        {
            var instance = Instantiate(playerControllerPrefab, transform);
            _playerController = instance.GetComponent<PlayerController>();
            _playerController.Setup(_characterSelectionList);
        }
    }

    private void InstantiatePoolController()
    {
        var poolPrefab = GetControllerFromPrefabList<PoolController>();
        if (poolPrefab != null && _poolController == null)
        {
            var instance = Instantiate(poolPrefab, transform);
            _poolController = instance.GetComponent<PoolController>();
            _poolController.Setup();
        }
    }

    private void HandleOnGameSceneSelectedIndex(int buildIndex)
    {
        SetGameStatus(GameStatus.Loading);
        SceneController.LoadScene(buildIndex);
        InstantiatePlayerController();
        InstantiatePoolController();        
        //UIController.InstantiatePlayerInGameUIController();
    }

    private void HandleOnUIBackToMainMenuFromMapaMundi()
    {
        if(SceneController != null)
            SceneController.LoadScene(ScenesBuildIndexes.MainMenu);
    }
}