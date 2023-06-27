using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class InputSystemController : ControllerUnit
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    public PlayerInputManager PlayerInputManager => _playerInputManager;

    public override void Setup()
    {
        base.Setup();

        _playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    public override void Dispose()
    {
        _playerInputManager = null;
    }

    public GameObject AddPlayerWithIDAndDevice(int playerIndex, GameObject prefab, InputDevice device = null)
    {
        _playerInputManager.playerPrefab = prefab;

        var playerInput = (device == null)? 
            _playerInputManager.JoinPlayer(playerIndex) : 
            _playerInputManager.JoinPlayer(playerIndex, -1, null, device);

        InputUser lastPlayerInputUser = InputUser.all[InputUser.all.Count - 1];
        
        for (int i = 1; i < lastPlayerInputUser.pairedDevices.Count; i++)
        {
            InputDevice lastDevice = lastPlayerInputUser.pairedDevices[i];
            lastPlayerInputUser.UnpairDevice(lastDevice);
        }

        var playersInputs = FindObjectsOfType<UnityEngine.InputSystem.PlayerInput>().ToList();
        var player = playersInputs.SingleOrDefault(input => input.playerIndex == playerIndex)?.gameObject;
        return player;
    }

    public void UnpairDevices()
    {
        foreach (var inputUser in InputUser.all)
        {
            inputUser.UnpairDevices();
        }
    }
}
