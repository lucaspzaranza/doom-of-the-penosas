using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;
using UnityEngine.SceneManagement;

public class GameController : Controller
{
    // Singleton instance
    private static GameController instance;

    // Props
    [SerializeField] private GameStatus _gameStatus;
    public GameStatus GameStatus => _gameStatus;

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
    private List<Penosas> _characterSelection;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            print("Destroying GameController duplicate...");
            Destroy(gameObject);
        }
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

    private void Update()
    {
        _gameModeState = GetGameMode();
    }

    private void EventHandlerSetup()
    {
        UIController.OnUISelectedCharacters += HandleOnUISelectedCharacters;
        SceneController.OnSceneLoaded += HandleOnSceneLoaded;
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;
        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

        UIController.OnUISelectedCharacters -= HandleOnUISelectedCharacters;

        SceneController.OnSceneLoaded -= HandleOnSceneLoaded;

        PlayerController.Dispose();
        UIController.Dispose();
        PoolController.Dispose();
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
        print("HandleOnUISelectedCharacters");
        UIController.DisposeLobbyController();

        foreach (var character in characterSelectionList)
        {
            // temporary
            print(character.ToString());
        }
        if(_characterSelection == null)
            _characterSelection = new List<Penosas>();
        _characterSelection = characterSelectionList.ToList();

        SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);

        UIController.InstantiateMapaMundiController();
    }

    private void InstantiateInGameControllers()
    {
        var playerControllerPrefab = GetControllerFromPrefabList<PlayerController>();
        if (playerControllerPrefab != null)
        {
            var instance = Instantiate(playerControllerPrefab, transform);
            _playerController = instance.GetComponent<PlayerController>();
            _playerController.Setup();
        }

        UIController.InstantiatePlayerInGameUIController();

        var poolPrefab = GetControllerFromPrefabList<PoolController>();
        if (playerControllerPrefab != null)
        {
            var instance = Instantiate(poolPrefab, transform);
            _poolController = instance.GetComponent<PoolController>();
            _poolController.Setup();
        }
    }

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
                InstantiateInGameControllers();
                InGameControllersSetup();
            }
            else if(GameStatus == GameStatus.Menu)
            {
                UIController.PlayerLobbyUIController.gameObject.SetActive(true);
            }
        }
    }
}