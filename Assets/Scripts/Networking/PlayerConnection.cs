using Mirror;
using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    [SyncVar] [SerializeField] private PlayerSelectionData _playerSelectionData;
    public PlayerSelectionData PlayerSelectionData
    {
        get => _playerSelectionData;
        private set => _playerSelectionData = value;
    }

    private void Start()
    {
        CmdGetNetworkArrow();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    [Server]
    public void InstantiatePlayerPrefab()
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(player, connectionToClient);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(player);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetNetworkArrow()
    {
        var networkArrow = FindObjectsOfType<NetworkArrow>()
            .SingleOrDefault(arrow => arrow.connectionToClient == netIdentity.connectionToClient);
        RpcSetNetworkArrow(networkArrow);
    }

    [ClientRpc]
    private void RpcSetNetworkArrow(NetworkArrow newArrow)
    {
        if (PlayerSelectionUIController.instance.TryGetSelectedPenosaFromButton(newArrow, out Penosas selectedPenosa))
        {
            PlayerSelectionData.NetworkArrow = newArrow;
            PlayerSelectionData.SelectedPenosa = selectedPenosa;
        }
    }
}