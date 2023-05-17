using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ControllerUnit
{
    // Events
    public Action OnGameOverCountdownTextIsNull;
    public Action<bool> OnCountdownActivation;

    // Vars
    [Space]
    [SerializeField] private Transform playerStartPosition = null;

    // Props
    [SerializeField] private List<PlayerData> _playersData = null;
    public List<PlayerData> PlayersData => _playersData;

    private Text _gameOverCountdownText;

    private void Update()
    {
        if (PlayersData.Count == 0) return;
        if (PlayersData[0].OnCountdown && PlayersData[0].Countdown >= 0)
            GameOverCountdown(0);
    }

    public override void Setup()
    {
        base.Setup();
        GetPlayersOnScene();
    }

    public override void Dispose()
    {
        _playersData = null;
    }

    private void GetPlayersOnScene()
    {
        var players = GameObject.FindGameObjectsWithTag(ConstantStrings.PlayerTag)
            .Select(player => player.GetComponent<Penosa>())
            .ToList();

        _playersData = new List<PlayerData>();
        float xOffset = 0f;
        players.ForEach(player =>
        {
            PlayersData.Add(player.PlayerData);
            player.transform.position = new Vector2(playerStartPosition.position.x + xOffset, playerStartPosition.position.y);
            xOffset++;
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
        if (_gameOverCountdownText == null)
            OnGameOverCountdownTextIsNull?.Invoke();
        OnCountdownActivation?.Invoke(true);
    }

    private void GameOverCountdown(byte ID)
    {
        PlayersData[ID].Countdown -= Time.deltaTime;
        if (PlayersData[ID].Countdown >= 0)
        {
            int currentCountdown = Mathf.FloorToInt(PlayersData[ID].Countdown);
            if (currentCountdown.ToString() != _gameOverCountdownText.text)
                _gameOverCountdownText.text = currentCountdown.ToString();

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown(InputStrings.Start)) // Respawn Penosa
            {
                PlayersData[ID].GameObject.SetActive(true);
                PlayersData[ID].Countdown = ConstantNumbers.CountdownSeconds;
                PlayersData[ID].OnCountdown = false;
                PlayersData[ID].Lives = PlayerConsts.initial_lives;
                PlayersData[ID].Player.ResetPlayerData();
                PlayersData[ID].Player.Inventory.ClearInventory();
                PlayersData[ID].Player.InitiateBlink();
                _gameOverCountdownText.text = ConstantNumbers.CountdownSeconds.ToString();
                OnCountdownActivation?.Invoke(false);
            }
        }
        else
        {
            _gameOverCountdownText.text = string.Empty;
            // Mais pra frente, adicionar funcionalidade de navegar de volta ao 
            // menu inicial.
        }
    }

    public void SetGameOverCountdownText(Text countdownTxt)
    {
        _gameOverCountdownText = countdownTxt;
    }
}
