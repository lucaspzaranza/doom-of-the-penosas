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
    public Action<byte, bool> OnCountdownActivation;
    public Action<bool> OnPlayerPause;
    public Action<byte> OnPlayerGameOver;
    public Action<List<Penosas>> OnPlayersExchanged;

    // Vars
    [Space]
    [SerializeField] private float _offsetX;

    // Props
    [SerializeField] private Vector2 _playerStartPosition;
    public Vector2 PlayerStartPosition => _playerStartPosition;

    [SerializeField] private InputSystemController _inputSystemController;
    public InputSystemController InputSystemController => _inputSystemController;

    [SerializeField] private List<PlayerDataPrefabs> _playerPrefabs;
    public List<PlayerDataPrefabs> PlayerPrefabs => _playerPrefabs;

    [SerializeField] private List<PlayerData> _playersData = null;
    public List<PlayerData> PlayersData => _playersData;

    [SerializeField] private List<GameObject> _rideArmors;

    public void Setup(IReadOnlyList<Penosas> characters, IReadOnlyList<InputDevice> selectedDevices = null)
    {
        base.Setup();

        if(_playersData.Count != characters.Count)
            ResetPlayerData();
       
        for (int i = 0; i < characters.Count; i++)
        {
            AddNewPlayerData(characters[i], i, selectedDevices[i]);
        }

        WalkTalk.OnRequestWalkTalkUse += HandleOnRequestWalkTalkUse;        
    }

    private void PlayersEventSetup(Penosa player)
    {
        player.OnPlayerLostAllContinues += InvokeGameOverEvent;
        player.OnPlayerLostAllLives += RemovePlayerFromScene;
        player.OnPlayerRespawn += RespawnPlayerAfterLostAllLives;
        player.OnPlayerRideArmor += HandleOnPlayerRideArmor;
        player.OnPlayerDeath += HandleOnPlayerDeath;
    }

    public void EventDispose()
    {
        foreach (var playerData in PlayersData)
        {
            if (playerData.Player == null) continue;

            playerData.Player.OnPlayerLostAllContinues -= InvokeGameOverEvent;
            playerData.Player.OnPlayerLostAllLives -= RemovePlayerFromScene;
            playerData.Player.OnPlayerRespawn -= RespawnPlayerAfterLostAllLives;
            playerData.Player.OnPlayerRideArmor -= HandleOnPlayerRideArmor;
            playerData.Player.OnPlayerDeath -= HandleOnPlayerDeath;
        }
    }

    public override void Dispose()
    {
        ResetPlayerData();
        EventDispose();

        WalkTalk.OnRequestWalkTalkUse -= HandleOnRequestWalkTalkUse;
    }

    private void AddNewPlayerData(Penosas characterToAdd, int idToAdd, InputDevice device = null)
    {
        PlayerData newPlayerData = new PlayerData(characterToAdd, idToAdd, device);
        SetPlayerProjectilesPrefabs(newPlayerData);

        var existingData = _playersData.
            SingleOrDefault(data => data.Character == characterToAdd && data.LocalID == idToAdd);

        if(existingData != null)
        {
            if(existingData.LocalID != idToAdd)
                existingData = newPlayerData;
        }
        else
        {
            if (_playersData.Count > 0 && idToAdd < _playersData.Count)
                _playersData.RemoveAt(idToAdd);
            _playersData.Insert(idToAdd, newPlayerData);
        }        
    }

    private PlayerDataPrefabs GetPlayerPrefabs(Penosas character) =>
        PlayerPrefabs.SingleOrDefault(prefab => prefab.Character == character);

    public void AddPlayers()
    {
        InstantiatePlayerInputController();

        foreach (var character in GetCharacterSelectionList())
        {
            var playerPrefabs = GetPlayerPrefabs(character);
            var playerData = PlayersData.SingleOrDefault(data => data.Character == character);

            if (playerData.GameOver)
            {
                if (playerData.Continues >= 1)
                {
                    playerData.GameOver = false;
                    playerData.Lives = PlayerConsts.Initial_Lives;
                }
                else
                    continue;
            }

            var newPlayer = _inputSystemController.
                AddPlayerWithIDAndDevice(playerData.LocalID, playerPrefabs.PlayerPrefab, playerData.InputDevice);

            if(newPlayer == null)
            {
                WarningMessages.CantAddPlayerMessage(playerPrefabs.Character.ToString());
                continue;
            }

            PlayerDataSetup(playerData, newPlayer);
        }
    }

    public void AddSinglePlayer(byte playerID, Penosas newCharacter)
    {
        var playerPrefabs = GetPlayerPrefabs(newCharacter);
        var playerData = PlayersData.SingleOrDefault(data => data.Character == newCharacter);

        var newPlayer = _inputSystemController.
                AddPlayerWithIDAndDevice(playerData.LocalID, playerPrefabs.PlayerPrefab, playerData.InputDevice);

        if (newPlayer == null)
            WarningMessages.CantAddPlayerMessage(playerPrefabs.Character.ToString());
        else
            PlayerDataSetup(playerData, newPlayer);
    }

    private void PlayerDataSetup(PlayerData playerData, GameObject newPlayer)
    {
        var newPlayerScript = newPlayer.GetComponent<Penosa>();
        GameController gameController = TryToGetGameControllerFromParent();

        playerData.SetPlayerScriptFromInstance(newPlayerScript);
        playerData.SetPlayerGameObjectFromInstance(newPlayer);
        playerData.InventoryDataSetup(newPlayerScript, gameController.IsNewGame);

        newPlayerScript.SetPlayerData(playerData);
        newPlayerScript.SetPlayerController(this);

        PlayersEventSetup(newPlayerScript);

        if (!gameController.IsNewGame) // It will load 1st and 2nd weapon level only between stage changing.
            newPlayerScript.Inventory.LoadInventoryData(playerData.InventoryData);

        SetPlayerStartPosition(newPlayer, playerData.LocalID);
    }
   
    public void ResetPlayerEquipmentData()
    {
        foreach (var playerData in _playersData)
        {
            if (playerData.GameOver) 
                continue;

            playerData._1stWeaponLevel = PlayerConsts.WeaponInitialLevel;
            playerData._2ndWeaponLevel = PlayerConsts.WeaponInitialLevel;
            playerData._1stWeaponAmmoProp = PlayerConsts._1stWeaponInitialAmmo;
            playerData._2ndWeaponAmmoProp = PlayerConsts._2ndWeaponInitialAmmo;
            playerData.Player.Life = PlayerConsts.Max_Life;
            playerData.Player.ArmorLife = 0;
            playerData.Continues = PlayerConsts.Continues;
        }
    }

    public void ResetSinglePlayerData(byte playerID)
    {
        _playersData[playerID]._1stWeaponLevel = PlayerConsts.WeaponInitialLevel;
        _playersData[playerID]._2ndWeaponLevel = PlayerConsts.WeaponInitialLevel;
        _playersData[playerID]._1stWeaponAmmoProp = PlayerConsts._1stWeaponInitialAmmo;
        _playersData[playerID]._2ndWeaponAmmoProp = PlayerConsts._2ndWeaponInitialAmmo;
        _playersData[playerID].Player.Life = PlayerConsts.Max_Life;
        _playersData[playerID].Player.ArmorLife = 0;
        _playersData[playerID].Continues = PlayerConsts.Continues;
    }

    private void SetPlayerProjectilesPrefabs(PlayerData playerData)
    {
        PlayerDataPrefabs prefabs = PlayerPrefabs.
            SingleOrDefault(prefab => prefab.Character == playerData.Character);
        playerData.SetProjectilesPrefabs(prefabs);
    }

    private void InstantiatePlayerInputController()
    {
        GameObject inputControllerPrefab = ChildControllersPrefabs.
            SingleOrDefault(prefab => prefab.GetComponent<InputSystemController>() != null);

        //if (inputControllerPrefab != null && _inputSystemController == null)
        if (inputControllerPrefab != null)
        {
            GameObject newInstance = Instantiate(inputControllerPrefab, transform);
            _inputSystemController = newInstance.GetComponent<InputSystemController>();
            _inputSystemController.Setup();
        }
    }

    public void RemovePlayerFromScene(byte playerID)
    {
        SetPlayerStartPosition(PlayersData[playerID].PlayerGameObject, playerID);
        PlayersData[playerID].Player.Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        PlayersData[playerID].Player.SetPlayerOnSceneAfterGameOver(false);
        PlayersData[playerID].Player.ResetPlayerData();
        PlayersData[playerID].Continues--;

        if (PlayersData[playerID].Continues <= 0)
            InvokeGameOverEvent(playerID);
        else
            OnCountdownActivation?.Invoke(playerID, true);
    }

    private void SetPlayerStartPosition(GameObject player, byte playerID)
    {
        player.transform.position = new Vector2
            (_playerStartPosition.x + (_offsetX * PlayersData[playerID].LocalID), _playerStartPosition.y);
    }

    public void RespawnPlayerAfterLostAllLives(byte playerID)
    {
        PlayersData[playerID].Player.SetPlayerOnSceneAfterGameOver(true);
        PlayersData[playerID].Lives = PlayerConsts.Initial_Lives;
        PlayersData[playerID].Player.Inventory.ClearInventory();
        PlayersData[playerID].Player.InitiateBlink();

        ReinstantiateRideArmorIfNecessary(playerID);
    }

    public void RemoveInputController()
    {
        InputSystemController.UnpairDevices();
        Destroy(InputSystemController.gameObject);
        _inputSystemController = null;
    }

    public GameObject RequestProjectileFromGameController(GameObject projectile)
    {
        return TryToGetGameControllerFromParent().GetProjectileFromPool(projectile);
    }

    public void InvokeGameOverEvent(byte playerID)
    {
        OnPlayerGameOver?.Invoke(playerID);
    }

    /// <summary>
    /// Solution to change character in a singleplayer game.
    /// </summary>
    /// <param name="playerID">The player local ID.</param>
    /// <param name="newCharacter">The character you want to choose.</param>
    public void ChangePlayerCharacter(byte playerID, Penosas newCharacter)
    {
        Vector2 oldPosition = PlayersData[playerID].Player.transform.position;
        bool isLeft = PlayersData[playerID].Player.IsLeft;

        Destroy(PlayersData[playerID].Player.gameObject);
        AddNewPlayerData(newCharacter, playerID, PlayersData[playerID].InputDevice);
        AddSinglePlayer(playerID, newCharacter);

        var newCharacters = PlayersData.Select(data => data.Character).ToList();
        TryToGetGameControllerFromParent().SelectCharacters(newCharacters);

        PlayersData[playerID].Player.transform.position = oldPosition;

        if (isLeft)
            PlayersData[playerID].Player.Flip();
    }
    
    /// <summary>
    /// It works with a 2-Player multiplayer logic only.
    /// </summary>
    public void ExchangePlayers()
    {
        if (TryToGetGameControllerFromParent().GameMode == GameMode.Singleplayer)
            return;

        InputSystemController.UnpairDevices();
        List<byte> ids = PlayersData.Select(data => data.LocalID).ToList();
        List<Vector3> positions = PlayersData.Select(data => data.Player.transform.position).ToList();

        PlayerData dataAux = PlayersData[0];
        PlayersData[0] = PlayersData[1];
        PlayersData[1] = dataAux;

        for (byte i = 0; i < PlayersData.Count; i++)
        {
            PlayersData[i].Player.gameObject.transform.localPosition = positions[i];
            PlayersData[i].LocalID = ids[i];
            InputSystemController.PairDeviceWithPlayer(i, PlayersData[i].InputDevice);
            Destroy(PlayersData[i].Player.CurrentGrenade);
        }

        InputDevice inputAux = PlayersData[0].InputDevice;
        PlayersData[0].InputDevice = PlayersData[1].InputDevice;
        PlayersData[1].InputDevice = inputAux;

        var prefabAux = _playerPrefabs[0];
        _playerPrefabs[0] = _playerPrefabs[1];
        _playerPrefabs[1] = prefabAux;
        OnPlayersExchanged?.Invoke(_playerPrefabs.Select(player => player.Character).ToList());
    }

    public void ResetPlayerData()
    {
        _playersData = new List<PlayerData>();
    }

    public void HandleOnPlayerRideArmor(byte playerID, RideArmor rideArmor, bool isEquipping)
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if(gameCtrl != null)
        {
            gameCtrl.UIController.PlayerInGameUIController
                .UpdateHUDWithRideArmor(playerID, rideArmor, isEquipping);
        }
    }

    public void HandleOnPlayerDeath(byte playerID)
    {
        if (_playersData[playerID].Lives <= 0)
            return;

        ReinstantiateRideArmorIfNecessary(playerID);
    }

    public void ReinstantiateRideArmorIfNecessary(byte playerID)
    {
        RideArmorType rideArmorRequired = TryToGetGameControllerFromParent()
            .StageController.RideArmorRequired;

        if (rideArmorRequired != RideArmorType.None)
        {
            GameObject rideArmorPrefab = _rideArmors
                .FirstOrDefault(armor => armor.GetComponent<RideArmor>().Type == rideArmorRequired);

            if (rideArmorPrefab != null)
            {
                GameObject instance = Instantiate(rideArmorPrefab,
                    _playersData[playerID].Player.transform.position, Quaternion.identity);
                RideArmor rideArmor = instance.GetComponent<RideArmor>();

                _playersData[playerID].Player.RideArmor(rideArmor);
                rideArmor.Required = true;
            }
        }
    }

    private bool HandleOnRequestWalkTalkUse()
    {
        return PlayersData.All(playerData => !playerData.Player.RideArmorEquipped);
    }
}
