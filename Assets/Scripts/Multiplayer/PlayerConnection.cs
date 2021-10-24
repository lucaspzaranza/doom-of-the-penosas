using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        print("OnStartLocalPlayer");
    }

    [Server]
    public void InstantiatePlayerPrefab()
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(player, connectionToClient);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(player);
    }
}
