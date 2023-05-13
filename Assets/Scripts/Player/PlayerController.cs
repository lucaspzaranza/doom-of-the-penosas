using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IController
{
    public const byte countdown = 10;

    [SerializeField] private Transform playerStartPosition = null;
    private Text gameOverCountdownText;

    public List<PlayerData> PlayersData => _playersData;
    [SerializeField] private List<PlayerData> _playersData = null;

    private void Update()
    {
        if (PlayersData.Count == 0) return;
        if (PlayersData[0].OnCountdown && PlayersData[0].Countdown >= 0)
            GameOverCountdown(0);
    }

    public void Setup()
    {
        GetPlayersOnScene();
    }

    public void Dispose()
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
