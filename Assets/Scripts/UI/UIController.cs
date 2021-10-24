using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public PlayerHUD[] huds = new PlayerHUD[2];

    [Header("Game Over")]
    public GameObject GameOverContainerObject;
    public GameObject Title;
    public GameObject Countdown;

    private GameObject[] players;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        GetPlayersScriptToHUDController();
        if (players.Length == 2)
            huds[1].gameObject.SetActive(true);
    }

    private void GetPlayersScriptToHUDController()
    {
        foreach (var player in players)
        {
            huds[player.GetComponent<Penosa>().PlayerData.LocalID].Player = player.GetComponent<Penosa>();
        }
    }
}