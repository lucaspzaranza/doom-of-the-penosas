using Mirror;
using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PenosasNetworkManager : NetworkManager
{
    private int playerCount = 0;
    private PlayerSelectionController selectionController;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        NetworkClient.AddPlayer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // Player 1
        if(playerCount == 0)
            InstantiateArrowEgg(0, conn);
        else if(playerCount == 1) // Player 2
            InstantiateArrowEgg(1, conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        playerCount--;
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        if (conn.identity.isClientOnly)
            PlayerSelectionUIController.instance.SetDisconnectedMenuActivation(true);
    }

    [Server]
    private void InstantiateArrowEgg(int index, NetworkConnection conn)
    {
        var playerArrow = Instantiate(spawnPrefabs[index]);
        NetworkServer.Spawn(playerArrow, conn);
        playerCount++;

        //var playerconnection = conn.identity.GetComponent<PlayerConnection>();
        //var networkArrow = playerArrow.GetComponent<NetworkArrowPosition>();
        //playerconnection.CmdSetNetworkArrow(networkArrow, index);
    }

    [Server]
    public GameObject InstantiatePlayer(Penosas chosenPlayer, NetworkConnection connectionToClient)
    {
        GameObject penosaPrefab = spawnPrefabs.FirstOrDefault(prefab => prefab.name == chosenPlayer.ToString());
        GameObject penosa = Instantiate(penosaPrefab, transform.position, Quaternion.identity);

        NetworkServer.Spawn(penosa, connectionToClient);

        return penosa;
    }

    public void StartGame()
    {
        ServerChangeScene("SampleScene");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
    }
}