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

    private void Update()
    {
        _gameModeState = GetGameMode();
    }

    public override void Dispose()
    {
        Penosa.OnPlayerDeath -= PlayerController.RemovePlayerFromScene;

        PlayerController.OnGameOverCountdownTextIsNull -= HandleOnGameOverCountdownTextIsNull;

        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;

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

    private void HandleUIControllerOnGameStart(IReadOnlyList<Penosas> characterSelectionList)
    {
        foreach (var character in characterSelectionList)
        {
            print(character.ToString());
        }

        //UIController.DisposeLobbyController();
        InstantiateInGameControllers();
        InGameControllersSetup();
    }

    private void InstantiateInGameControllers()
    {
        var playerController = ChildControllers.FirstOrDefault(ctrl => ctrl.GetComponent<PlayerController>());
        if (playerController != null)
        {
            var playerControllerInstance = Instantiate(playerController.gameObject, transform);
            _playerController = playerControllerInstance.GetComponent<PlayerController>();
        }

        var playerInGameUIController = UIController.ChildControllers.
            FirstOrDefault(ctrl => ctrl.GetComponent<PlayerInGameUIController>());

        if (playerInGameUIController != null)
        {
            var playerInGameUIControllerInstance = Instantiate(playerInGameUIController.gameObject, UIController.transform);
            var inGameUIController = playerInGameUIControllerInstance.GetComponent<PlayerInGameUIController>();
            UIController.SetInGameUIController(inGameUIController);
        }

        var poolController = ChildControllers.FirstOrDefault(ctrl => ctrl.GetComponent<PoolController>());
        if (poolController != null)
        {
            var poolControllerInstance = Instantiate(poolController.gameObject, transform);
            _poolController = poolControllerInstance.GetComponent<PoolController>();
        }
    }

    private void InGameControllersSetup()
    {
        _playerController.Setup();
        _poolController.Setup();
        UIController.InGameUIControllerSetup();
    }
}