using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameUIController : ControllerUnit, IUIController
{
    [Space]
    [SerializeField] private PlayerHUD[] _huds = new PlayerHUD[ConstantNumbers.NumberOfPlayers];
    public PlayerHUD[] HUDs => _huds;

    [Header("Game Over")]
    public GameObject GameOverContainerObject;
    public GameObject Title;
    public GameObject Countdown;

    private GameObject[] players;

    public override void Setup()
    {
        _parentController = GetComponentInParent<UIController>();

        players = GameObject.FindGameObjectsWithTag(ConstantStrings.PlayerTag);
        GetPlayersScriptToHUDController();

        //if (players.Length == 2)
        if (GetGameMode() == GameMode.Multiplayer)
            HUDs[ConstantNumbers.NumberOfPlayers - 1].gameObject.SetActive(true);
    }

    //public override void Dispose()
    //{

    //}

    private void GetPlayersScriptToHUDController()
    {
        foreach (var player in players)
        {
            HUDs[player.GetComponent<Penosa>().PlayerData.LocalID].Player = player.GetComponent<Penosa>();
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
