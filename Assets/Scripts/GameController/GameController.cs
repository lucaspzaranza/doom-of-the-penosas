using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

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

    private IReadOnlyList<InputDevice> _selectedDevices;
    public IReadOnlyList<InputDevice> SelectedDevices => _selectedDevices;

    [Header("Controllers")]
    [SerializeField] private PersistenceController _persistenceController;
    public PersistenceController PersistenceController => _persistenceController;

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;

    [SerializeField] private UIController _uiController;
    public UIController UIController => _uiController;

    [SerializeField] private PoolController _poolController;
    public PoolController PoolController => _poolController;

    [SerializeField] private SceneController _sceneController;
    public SceneController SceneController => _sceneController;

    public bool GameIsPaused => Time.timeScale == 0f;

    private bool IsSingleInstance => instance == this;

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
        PersistenceController.Setup();

        EventHandlerSetup();       
    }

    private void EventHandlerSetup()
    {
        UIController.OnUISelectedCharacters += HandleOnUISelectedCharacters;
        UIController.OnUIGameModeSelected += SetGameMode;
        UIController.OnUISetNewGame += SetNewGame;
        UIController.OnUIGameSelectedSceneIndex += HandleOnGameSceneSelectedIndex;
        UIController.OnUIBackToMainMenuFromMapaMundi += HandleOnUIBackToMainMenuFromMapaMundi;
        UIController.OnUISelectedDevices += HandleOnUISelectedDevices;

        SceneController.OnSceneLoaded += HandleOnSceneLoaded;

        PauseMenu.OnResume += ResumeGame;
        PauseMenu.OnBackToMainMenu += BackToMainMenuButton;
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;
        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;
        PlayerController.OnPlayerPause -= HandleOnPlayerPause;

        UIController.OnUISelectedCharacters -= HandleOnUISelectedCharacters;
        UIController.OnUIGameModeSelected -= SetGameMode;
        UIController.OnUISetNewGame -= SetNewGame;
        UIController.OnUIGameSelectedSceneIndex -= HandleOnGameSceneSelectedIndex;
        UIController.OnUIBackToMainMenuFromMapaMundi -= HandleOnUIBackToMainMenuFromMapaMundi;
        UIController.OnUISelectedDevices -= HandleOnUISelectedDevices;

        SceneController.OnSceneLoaded -= HandleOnSceneLoaded;

        PauseMenu.OnResume -= ResumeGame;
        PauseMenu.OnBackToMainMenu -= BackToMainMenuButton;

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

    private void HandleOnSceneLoaded(Scene scene)
    {
        if(scene.buildIndex == ScenesBuildIndexes.MainMenu)
        {
            if(GameStatus == GameStatus.Loading)
            {
                print("SetGameStatus(GameStatus.Menu)");
                SetGameStatus(GameStatus.Menu);
            }
            else if(GameStatus == GameStatus.Menu)
                UIController.PlayerLobbyUIController.gameObject.SetActive(true);
        }
        else if(scene.buildIndex == ScenesBuildIndexes.MapaMundi)
        {
            //int completedStages = IsNewGame? 0 : PersistenceController.LoadCompletedStages();
            //UIController.MapaMundiController.ActivateStageLoaders(completedStages);
        }
        else if(scene.buildIndex == ScenesBuildIndexes._1stStage)
        {
            if (GameStatus == GameStatus.Loading)
            {
                SetGameStatus(GameStatus.InGame);
                PlayerController.AddPlayers();

                if(IsNewGame)
                {
                    print("Saving game progress from the beginning...");
                    PersistenceController.SaveCompletedStages(0);
                }
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
            _playerController.Setup(_characterSelectionList, _selectedDevices);

            _playerController.OnGameOverCountdownTextIsNull += HandleOnGameOverCountdownTextIsNull;
            _playerController.OnCountdownActivation += HandleOnCoutdownActivation;
            _playerController.OnPlayerPause += HandleOnPlayerPause;

            Penosa.OnPlayerDeath += PlayerController.RemovePlayerFromScene;

            //_playerController.Setup();
            //_poolController.Setup();
            //UIController.InGameUIControllerSetup();
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

    private void HandleOnUISelectedDevices(IReadOnlyList<InputDevice> devices)
    {
        _selectedDevices = devices;
    }

    public void HandleOnPlayerPause(bool val)
    {
        Time.timeScale = val ? 0f : 1f;
        UIController.PauseMenuActivation(val);
    }

    public void ResumeGame()
    {
        if (GameIsPaused)
            PlayerController.OnPlayerPause?.Invoke(false);
    }

    public void BackToMainMenuButton()
    {
        PlayerController.OnPlayerPause?.Invoke(false);
        PlayerController.RemoveInputController();
        Destroy(PlayerController.gameObject);
        _playerController = null;

        SetGameStatus(GameStatus.Menu);
        SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);
    }

    public GameObject GetProjectileFromPool(GameObject projectile)
    {
        return PoolController.GetProjectile(projectile);
    }

    public void HandleOnMapaMundiReferencesLoaded()
    {
        int completedStages = IsNewGame ? 0 : PersistenceController.LoadCompletedStages();
        UIController.MapaMundiController.ActivateStageLoaders(completedStages);
    }
}