using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InputStrings
{
    public const string Jump = "Jump";
    public const string Fire1 = "Fire1";
    public const string Fire2 = "Fire2";
    public const string Fire3 = "Fire3";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string ChangeSpecialItem = "ChangeSpecialItem";
    public const string Start = "Start";
}

public class GameController : MonoBehaviour
{
    #region Constants

    public const byte countdown = 10;

    #endregion

    #region Variables
    public static GameController instance;
    [SerializeField] private List<PlayerData> _playersData = null;
    [SerializeField] private Transform playerStartPosition = null;
    private Text gameOverCountdownText;

    #endregion

    #region #Properties

    public List<PlayerData> PlayersData => _playersData;

    #endregion

    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        GetPlayersOnScene();

        InitiateProjectilesPools();

        // Not working yet ;~
        //var PlayerSelectionController = PlayerSelectionInputController.instance;

        //if(PlayerSelectionController?.PlayerCount == 1)
        //{
        //    var penosa = PlayerSelectionController.PlayersSelectionData[0].Prefab;
        //    Instantiate(penosa, playerStartPosition.position, Quaternion.identity);
        //}
    }

    void Update()
    {
        if (PlayersData[0].OnCountdown && PlayersData[0].Countdown >= 0)
            GameOverCountdown(0);
    }

    private void InitiateProjectilesPools()
    {
        StartCoroutine(ObjectPool.instance.InitializePool("Egg Shot"));
        StartCoroutine(ObjectPool.instance.InitializePool("Big Egg Shot"));
        StartCoroutine(ObjectPool.instance.InitializePool("Grenade"));
        StartCoroutine(ObjectPool.instance.InitializePool("Shuriken"));
        StartCoroutine(ObjectPool.instance.InitializePool("Fuuma Shuriken"));
    }

    private void GetPlayersOnScene()
    {
        var players = GameObject.FindGameObjectsWithTag("Player")
            .Select(player => player.GetComponent<Penosa>())
            .ToList();
        _playersData = new List<PlayerData>();
        players.ForEach(player =>
        {
            PlayersData.Add(player.PlayerData);
        });
    }

    public void RemovePlayerFromScene(byte ID)
    {
        PlayersData[ID].GameObject.SetActive(false);
        PlayersData[ID].Continues--;

        if (PlayersData[ID].Continues == 0)
        {
            // Game over...
        }
        else
        {
            // Call Respawn Countdown  
            RespawnCountdown(ID);
        }
    }

    private void RespawnCountdown(byte ID)
    {
        PlayersData[ID].OnCountdown = true;
        if (gameOverCountdownText == null)
            gameOverCountdownText = UIController.instance.Countdown.GetComponent<Text>();
        UIController.instance.GameOverContainerObject.gameObject.SetActive(true);
    }

    private void GameOverCountdown(byte ID)
    {
        PlayersData[ID].Countdown -= Time.deltaTime;
        if (PlayersData[ID].Countdown >= 0)
        {
            int currentCountdown = Mathf.FloorToInt(PlayersData[ID].Countdown);
            if (currentCountdown.ToString() != gameOverCountdownText.text)
                gameOverCountdownText.text = currentCountdown.ToString();

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown(InputStrings.Start)) // Respawn Penosa
            {
                PlayersData[ID].GameObject.SetActive(true);
                PlayersData[ID].Countdown = countdown;
                PlayersData[ID].OnCountdown = false;
                PlayersData[ID].Lives = PlayerConsts.initial_lives;
                PlayersData[ID].Player.ResetPlayerData();
                PlayersData[ID].Player.Inventory.ClearInventory();
                PlayersData[ID].Player.InitiateBlink();
                gameOverCountdownText.text = countdown.ToString();
                UIController.instance.GameOverContainerObject.gameObject.SetActive(false);
            }
        }
        else
        {
            gameOverCountdownText.text = string.Empty;
            // Mais pra frente, adicionar funcionalidade de navegar de volta ao 
            // menu inicial.
        }
    }
}