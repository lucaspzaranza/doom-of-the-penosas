using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;

public class GameController : Controller
{
    [Header("Game General Data")]
    // variable to reflect the Controller Game Mode only in the Editor. 
    [SerializeField] private GameMode _gameModeState;
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

    void Start()
    {
        Setup();
        DontDestroyOnLoad(gameObject);
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
        UIController.OnGameStart += HandleUIControllerOnGameStart;
        SceneController.OnSceneLoaded += HandleOnSceneLoaded;
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;
        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

        UIController.OnGameStart -= HandleUIControllerOnGameStart;

        SceneController.OnSceneLoaded -= HandleOnSceneLoaded;

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

    private void HandleUIControllerOnGameStart(IReadOnlyList<Penosas> characterSelectionList)
    {
        foreach (var character in characterSelectionList)
        {
            print(character.ToString());
        }

        UIController.DisposeLobbyController();        
        SceneController.LoadScene(1);
    }

    private void InstantiateInGameControllers()
    {
        var playerController = ChildControllersPrefabs.SingleOrDefault(ctrl => ctrl.GetComponent<PlayerController>());
        if (playerController != null)
        {
            var playerControllerInstance = Instantiate(playerController, transform);
            _playerController = playerControllerInstance.GetComponent<PlayerController>();
        }

        var playerInGameUIController = UIController.ChildControllersPrefabs.
            SingleOrDefault(ctrl => ctrl.GetComponent<PlayerInGameUIController>());

        if (playerInGameUIController != null)
        {
            var playerInGameUIControllerInstance = Instantiate(playerInGameUIController, UIController.transform);
            var inGameUIController = playerInGameUIControllerInstance.GetComponent<PlayerInGameUIController>();
            UIController.SetInGameUIController(inGameUIController);
        }

        var poolController = ChildControllersPrefabs.SingleOrDefault(ctrl => ctrl.GetComponent<PoolController>());
        if (poolController != null)
        {
            var poolControllerInstance = Instantiate(poolController, transform);
            _poolController = poolControllerInstance.GetComponent<PoolController>();
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

    private void HandleOnSceneLoaded()
    {
        InstantiateInGameControllers();
        InGameControllersSetup();
    }
}