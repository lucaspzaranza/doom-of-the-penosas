using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SharedData.Enumerations;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine.TextCore.Text;

public class GameController : Controller
{
    #region Vars and Props
    // Singleton instance
    private static GameController instance;

    // Props
    [SerializeField] private GameMode _gameMode;
    /// <summary>
    /// Returns if game is singleplayer or multiplayer.
    /// </summary>
    public GameMode GameMode => _gameMode;

    [SerializeField] private GameStatus _gameStatus;
    /// <summary>
    /// Returns if game is in menu, in game, loading, cutscene, etc.
    /// </summary>
    public GameStatus GameStatus => _gameStatus;

    [SerializeField] private bool _isNewGame;
    public bool IsNewGame => _isNewGame;

    private List<Penosas> _characterSelectionList;
    public IReadOnlyList<Penosas> CharacterSelectionList => _characterSelectionList;

    private IReadOnlyList<InputDevice> _selectedDevices;
    public IReadOnlyList<InputDevice> SelectedDevices => _selectedDevices;

    public bool GameIsPaused => Time.timeScale == 0f;

    private bool IsSingleInstance => instance == this;

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

    [SerializeField] private StageController _stageController;
    public StageController StageController => _stageController;

    #endregion

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
        UIController.OnCountdownIsOver += GameOver;

        SceneController.OnSceneLoaded += HandleOnSceneLoaded;

        PauseMenu.OnResume += ResumeGame;
        PauseMenu.OnBackToMainMenu += BackToMainMenuButton;
        WalkTalk.OnWalkTalk += HandleOnWalkTalk;
        Enemy.OnEnemyDeath += HandleOnEnemyDefeated;
    }

    public override void Dispose()
    {
        PlayerController.OnCountdownActivation -= HandleOnCoutdownActivation;
        PlayerController.OnPlayerPause -= HandleOnPlayerPause;
        PlayerController.OnPlayerGameOver -= GameOver;
        PlayerController.OnPlayersExchanged -= HandleOnPlayersExchanged;

        UIController.OnUISelectedCharacters -= HandleOnUISelectedCharacters;
        UIController.OnUIGameModeSelected -= SetGameMode;
        UIController.OnUISetNewGame -= SetNewGame;
        UIController.OnUIGameSelectedSceneIndex -= HandleOnGameSceneSelectedIndex;
        UIController.OnUIBackToMainMenuFromMapaMundi -= HandleOnUIBackToMainMenuFromMapaMundi;
        UIController.OnUISelectedDevices -= HandleOnUISelectedDevices;
        UIController.OnCountdownIsOver -= GameOver;

        SceneController.OnSceneLoaded -= HandleOnSceneLoaded;

        PauseMenu.OnResume -= ResumeGame;
        PauseMenu.OnBackToMainMenu -= BackToMainMenuButton;
        WalkTalk.OnWalkTalk -= HandleOnWalkTalk;
        Enemy.OnEnemyDeath -= HandleOnEnemyDefeated;

        PlayerController.Dispose();
        UIController.Dispose();
        PoolController.Dispose();
    }

    /// <summary>
    /// Function to set if the game will be Singleplayer or Multiplayer.
    /// </summary>
    /// <param name="newGameMode"></param>
    private void SetGameMode(GameMode newGameMode)
    {
        _gameMode = newGameMode;
    }

    private void SetNewGame(bool val)
    {
        _isNewGame = val;
    }

    /// <summary>
    /// Function to set the main game status, if it's loading a scene, at some menu, or in game, etc.
    /// </summary>
    /// <param name="gameStatus"></param>
    public void SetGameStatus(GameStatus gameStatus)
    {
        _gameStatus = gameStatus;
    }

    public void GetAllControllers()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    private void HandleOnCoutdownActivation(byte playerID, bool val)
    {
        UIController.CountdownActivation(playerID, val);
    }
   
    public void SelectCharacters(IReadOnlyList<Penosas> characterSelectionList)
    {
        if (_characterSelectionList == null)
            _characterSelectionList = new List<Penosas>();
        _characterSelectionList = characterSelectionList.ToList();
    }

    private void HandleOnUISelectedCharacters(IReadOnlyList<Penosas> characterSelectionList)
    {
        UIController.DisposeLobbyController();
        SelectCharacters(characterSelectionList);
        SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);

        UIController.InstantiateMapaMundiController();
    }

    private void HandleOnSceneLoaded(Scene scene)
    {
        if(scene.buildIndex == ScenesBuildIndexes.MainMenu && GameStatus == GameStatus.Loading)
        {
            SetGameStatus(GameStatus.Menu);

            if (!UIController.PlayerLobbyUIController.gameObject.activeInHierarchy)
                UIController.PlayerLobbyUIController.gameObject.SetActive(true);
        }
        else if (scene.buildIndex == ScenesBuildIndexes.MapaMundi)
        {
            InstantiateStageController();
        }
        else if(scene.buildIndex >= ScenesBuildIndexes._1stStage && 
        scene.buildIndex <= ScenesBuildIndexes._6thStage && 
        GameStatus == GameStatus.Loading)
        {
            UIController.SelectGameSceneCanvas();
            PutPlayerOnStage(scene);
        }
    }

    private void PutPlayerOnStage(Scene scene)
    {
        SetGameStatus(GameStatus.InGame);
        PlayerController.AddPlayers();

        if (scene.buildIndex == ScenesBuildIndexes._1stStage && IsNewGame)
        {
            WarningMessages.SavingProgressFromTheBeggining();
            PersistenceController.SaveCompletedStages(0);
            StageController.ResetAllStagesClear();
        }

        StageController.SetCurrentStageSO(StageController.Stages.SingleOrDefault(
           stage => stage.SceneIndex == scene.buildIndex));

        if (!IsNewGame)
            StageController.SetStagesClearFromTo(PersistenceController.LoadCompletedStages());

        UIController.InstantiatePlayerInGameUIController();
    }

    private void InstantiatePlayerController()
    {
         if (IsNewGame && _playerController != null)
            RemovePlayerController();

        var playerControllerPrefab = GetControllerFromPrefabList<PlayerController>();

        if (IsNewGame || (playerControllerPrefab != null && _playerController == null))
        {
            var instance = Instantiate(playerControllerPrefab, transform);
            _playerController = instance.GetComponent<PlayerController>();
            _playerController.Setup(_characterSelectionList, _selectedDevices);

            _playerController.OnCountdownActivation += HandleOnCoutdownActivation;
            _playerController.OnPlayerPause += HandleOnPlayerPause;            
            _playerController.OnPlayerGameOver += GameOver;            
            _playerController.OnPlayersExchanged += HandleOnPlayersExchanged;            
        }
        else
            _playerController.Setup(_characterSelectionList, _selectedDevices);
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

    private void InstantiateStageController()
    {
        if (_stageController != null)
            return;

        var stagePrefab = GetControllerFromPrefabList<StageController>();
        if (_stageController == null && stagePrefab != null)
        {
            var instance = Instantiate(stagePrefab, transform);
            _stageController = instance.GetComponent<StageController>();
            _stageController.Setup();
            _stageController.OnStageClear += HandleOnStageClear;
        }
    }

    private IEnumerator WaitAndLoadNextStage(int nextStageIndex)
    {
        PlayerController.RemoveInputController();
        yield return new WaitForSeconds(ConstantNumbers.TimeToShowStageClearTxt);

        SetGameStatus(GameStatus.Loading);
        SceneController.LoadScene(nextStageIndex);
    }

    private void HandleOnStageClear(StageSO currentStageSO)
    {
        if(IsNewGame)
            SetNewGame(false);

        if(!currentStageSO.StageClear)
        {
            int completedStages = PersistenceController.LoadCompletedStages() + 1;
            PersistenceController.SaveCompletedStages(completedStages);
        }

        UIController.PlayerInGameUIController.Dispose();
        StartCoroutine(WaitAndLoadNextStage(currentStageSO.SceneIndex + 1));
    }

    private void HandleOnGameSceneSelectedIndex(int buildIndex)
    {
        SetGameStatus(GameStatus.Loading);
        SceneController.LoadScene(buildIndex);
        InstantiatePlayerController();
        InstantiatePoolController();       
    }

    private void HandleOnUISelectedDevices(IReadOnlyList<InputDevice> devices)
    {
        _selectedDevices = devices;
    }

    public void HandleOnPlayerPause(bool val)
    {
        SetGameStatus(val ? GameStatus.Menu : GameStatus.InGame);
        Time.timeScale = val ? 0f : 1f;
        UIController.PauseMenuActivation(val);
    }

    public void ResumeGame()
    {
        if (GameIsPaused)
            PlayerController.OnPlayerPause?.Invoke(false);
    }

    private void HandleOnUIBackToMainMenuFromMapaMundi()
    {
        if (SceneController != null)
            SceneController.LoadScene(ScenesBuildIndexes.MainMenu);
    }

    public void BackToMainMenuButton()
    {
        PlayerController.OnPlayerPause?.Invoke(false);
        BackToMainMenuFromStage();
    }

    private void BackToMainMenuFromStage()
    {
        PlayerController.RemoveInputController();

        if (IsNewGame)
            RemovePlayerController();
        else
        {
            PlayerController.EventDispose();
            PlayerController.ResetPlayerEquipmentData();
        }

        UIController.PlayerInGameUIController.Dispose();
        SceneController.LoadScene(ScenesBuildIndexes.MapaMundi);
    }

    private void RemovePlayerController()
    {
        _playerController.EventDispose();
        Destroy(PlayerController.gameObject);
        _playerController = null;
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

    private IEnumerator ActivateGameOverAndReturnToMapaMundi()
    {
        UIController.GameOverActivation();
        yield return new WaitForSeconds(ConstantNumbers.TimeToReturnToMapaMundiAfterGameOver);
        BackToMainMenuFromStage();
    }

    public void GameOver(byte playerID)
    {
        PlayerController.PlayersData[playerID].GameOver = true;

        if(GameMode == GameMode.Singleplayer)
        {
            StartCoroutine(nameof(ActivateGameOverAndReturnToMapaMundi));

            if (PlayerController.PlayersData[playerID].Continues <= 0) // Restore one player data
                PlayerController.ResetSinglePlayerData(playerID);
        }
        else
        {
            bool allPlayersAreGameOver = PlayerController.PlayersData.All(playerData => playerData.GameOver);
            if(allPlayersAreGameOver)
            {
                StartCoroutine(nameof(ActivateGameOverAndReturnToMapaMundi));

                foreach (var player in PlayerController.PlayersData)
                {
                    PlayerController.ResetSinglePlayerData(player.LocalID);
                }
            }
            else
                UIController.PlayerInGameUIController.SetGameOverContainerOnPlayerActive(playerID, true);
        }
    }

    public void HandleOnWalkTalk(byte playerID)
    {
        int complementaryCharacter = SharedFunctions.GetComplementaryIndex((int)CharacterSelectionList[playerID]);
        Penosas inverseCharacter = (Penosas)complementaryCharacter;

        if (GameMode == GameMode.Singleplayer)
            PlayerController.ChangePlayerCharacter(playerID, inverseCharacter);
        else
            PlayerController.ExchangePlayers();

        UIController.PlayerInGameUIController.DestroyAllHUDs();
        UIController.PlayerInGameUIController.CreatePlayersHUDs(PlayerController.PlayersData);
    }

    public void HandleOnPlayersExchanged(IReadOnlyList<Penosas> characters)
    {
        _characterSelectionList = new List<Penosas>(characters);
    }

    public void HandleOnEnemyDefeated(Enemy defeatedEnemy)
    {
        if (defeatedEnemy.IsBoss)
            print("You defeated a boss! Congratulations.");
    }
}