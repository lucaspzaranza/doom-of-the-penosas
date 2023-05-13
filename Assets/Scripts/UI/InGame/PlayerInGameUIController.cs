using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameUIController : MonoBehaviour, IUIController
{
    public PlayerHUD[] huds = new PlayerHUD[2];

    [Header("Game Over")]
    public GameObject GameOverContainerObject;
    public GameObject Title;
    public GameObject Countdown;

    private GameObject[] players;

    public void Setup()
    {
        players = GameObject.FindGameObjectsWithTag(ConstantStrings.PlayerTag);
        GetPlayersScriptToHUDController();
        if (players.Length == 2)
            huds[1].gameObject.SetActive(true);
    }

    public void Dispose()
    {

    }

    private void GetPlayersScriptToHUDController()
    {
        foreach (var player in players)
        {
            huds[player.GetComponent<Penosa>().PlayerData.LocalID].Player = player.GetComponent<Penosa>();
        }
    }
    
    public Text GetCountdownText()
    {
        return Countdown.GetComponent<Text>();
    }

    public void GameOverContainerActivation(bool val)
    {
        GameOverContainerObject.gameObject.SetActive(val);
    }
}
