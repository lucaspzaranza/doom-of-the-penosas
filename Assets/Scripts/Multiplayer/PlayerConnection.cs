using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    [SyncVar] [SerializeField] private NetworkArrowPosition _networkArrow;
    public NetworkArrowPosition NetworkArrow => _networkArrow;

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
    public void CmdSetNetworkArrow(NetworkArrowPosition arrow, int playerIndex)
    {
        _networkArrow = arrow;
        PlayerSelectionUIController.instance.PlayerDataList[playerIndex].NetworkArrow = _networkArrow;
    }
}
