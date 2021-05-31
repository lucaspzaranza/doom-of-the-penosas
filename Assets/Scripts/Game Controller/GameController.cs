using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputStrings
{
    public const string Jump = "Jump";
    public const string Fire1 = "Fire1";
    public const string Fire2 = "Fire2";
    public const string Fire3 = "Fire3";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string ChangeSpecialItem = "ChangeSpecialItem";
}

public class GameController : MonoBehaviour
{
    #region Constants

    public const byte countdown = 10;

    #endregion

    #region Variables
    public static GameController instance;
    [SerializeField] private List<PlayerData> _playersData = null;

    #endregion

    #region #Properties

    public List<PlayerData> PlayersData => _playersData;

    #endregion

    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        GetPlayersOnScene();
    }

    void Update()
    {
        if (PlayersData[0].OnCountdown && PlayersData[0].Countdown >= 0)
        {
            Countdown(0);
        }
    }

    private void GetPlayersOnScene()
    {
        var players = GameObject.FindGameObjectsWithTag("Player")
            .Select(player => player.GetComponent<Penosa>())
            .ToList<Penosa>();
        _playersData = new List<PlayerData>();
        byte id = 0;
        players.ForEach(player =>
        {
            PlayersData.Add(player.PlayerData);
            PlayersData[id].ID = id;
            id++;
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
        // Make UI HUD Countdown
    }

    private void Countdown(byte ID)
    {
        PlayersData[ID].Countdown -= Time.deltaTime;
        if (PlayersData[ID].Countdown >= 0)
        {
            print(Mathf.FloorToInt(PlayersData[ID].Countdown)); // Change this to HUD UI logic
            if (Input.GetKeyDown(KeyCode.Space)) // Respawn Penosa
            {
                print("Volta, menino...");
                PlayersData[ID].GameObject.SetActive(true);
                PlayersData[ID].Countdown = countdown;
                PlayersData[ID].OnCountdown = false;
                PlayersData[ID].Lives = PlayerConsts.initial_lives;
                PlayersData[ID].Player.ResetPlayerData();
            }
        }
        else
            print("Game over");
    }

    void DontDestroyOnLoad()
    {

    }
}