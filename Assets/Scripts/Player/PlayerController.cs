using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerController : ControllerUnit
{
    // Events
    public Action OnGameOverCountdownTextIsNull;
    public Action<bool> OnCountdownActivation;
    public Action<bool> OnPlayerPause;

    // Vars
    [Space]
    [SerializeField] private Vector2 _playerStartPosition;
    [SerializeField] private float _offsetX;

    // Props
    [SerializeField] private InputSystemController _inputSystemController;
    public InputSystemController InputSystemController => _inputSystemController;

    [SerializeField] private List<PlayerDataPrefabs> _playerPrefabs;
    public List<PlayerDataPrefabs> PlayerPrefabs => _playerPrefabs;

    [SerializeField] private List<PlayerData> _playersData = null;
    public List<PlayerData> PlayersData => _playersData;

    private Text _gameOverCountdownText;

    private void Update()
    {
        if (PlayersData.Count == 0) return;
        if (PlayersData[0].OnCountdown && PlayersData[0].Countdown >= 0)
            GameOverCountdown(0);
    }
   
    public void Setup(IReadOnlyList<Penosas> characters, IReadOnlyList<InputDevice> selectedDevices = null)
    {
        base.Setup();

        for (int i = 0; i < characters.Count; i++)
        {
            AddNewPlayerData(characters[i], selectedDevices[i]);
        }
    }

    public override void Dispose()
    {
        _playersData = null;
    }

    private void AddNewPlayerData(Penosas characterToAdd, InputDevice device = null)
    {
        //print($"Adding the {characterToAdd} to the PlayerDataController with Local ID {_playersData.Count}...");

        PlayerData newPlayerData = new PlayerData(characterToAdd, _playersData.Count, device);
        PlayerDataPrefabs prefabs = PlayerPrefabs.
            SingleOrDefault(prefab => prefab.Character == characterToAdd);
        newPlayerData.SetProjectilesPrefabs(prefabs);

        _playersData.Add(newPlayerData);
    }

    public void AddPlayers()
    {
        InstantiatePlayerInputController();

        foreach (var character in GetCharacterSelectionList())
        {
            var playerPrefab = PlayerPrefabs.SingleOrDefault(prefab => prefab.Character == character);
            var playerData = PlayersData.SingleOrDefault(data => data.Character == character);

            var newPlayer = _inputSystemController.
                AddPlayerWithIDAndDevice(playerData.LocalID, playerPrefab.PlayerPrefab, playerData.InputDevice);

            if(newPlayer == null)
            {
                WarningMessages.CantAddPlayerMessage(playerPrefab.Character.ToString());
                return;
            }

            var newPlayerScript = newPlayer.GetComponent<Penosa>();

            playerData.SetPlayerScriptFromInstance(newPlayerScript);
            playerData.SetPlayerGameObjectFromInstance(newPlayer);

            newPlayerScript.SetPlayerData(playerData);
            newPlayerScript.SetPlayerController(this);

            newPlayer.transform.position = new Vector2
                (_playerStartPosition.x + (_offsetX * playerData.LocalID), _playerStartPosition.y);
        }
    }

    private void InstantiatePlayerInputController()
    {
        GameObject inputControllerPrefab = ChildControllersPrefabs.
            SingleOrDefault(prefab => prefab.GetComponent<InputSystemController>() != null);

        if (inputControllerPrefab != null)
        {
            GameObject newInstance = Instantiate(inputControllerPrefab, transform);
            _inputSystemController = newInstance.GetComponent<InputSystemController>();
            _inputSystemController.Setup();
        }
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
            player.transform.position = new Vector2(_playerStartPosition.x + xOffset, _playerStartPosition.y);
            xOffset++;
        });
    }

    public void RemovePlayerFromScene(byte ID)
    {
        PlayersData[ID].PlayerGameObject.SetActive(false);
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
                PlayersData[ID].PlayerGameObject.SetActive(true);
                PlayersData[ID].Countdown = ConstantNumbers.CountdownSeconds;
                PlayersData[ID].OnCountdown = false;
                PlayersData[ID].Lives = PlayerConsts.Initial_Lives;
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

    public void RemoveInputController()
    {
        InputSystemController.UnpairDevices();
        Destroy(InputSystemController.gameObject);
        _inputSystemController = null;
    }

    public GameObject RequestProjectileFromGameController(GameObject projectile)
    {
        return ((GameController)_parentController).GetProjectileFromPool(projectile);
    }
}
