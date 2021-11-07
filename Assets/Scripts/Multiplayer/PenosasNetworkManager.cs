using Mirror;
using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class PenosasNetworkManager : NetworkManager
{
    private int playerCount = 0;

    [SerializeField] private List<PlayerConnection> _playerConnections;
    public List<PlayerConnection> PlayerConnections => _playerConnections;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _playerConnections = new List<PlayerConnection>();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        NetworkClient.AddPlayer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerConnections.Add(conn.identity.gameObject.GetComponent<PlayerConnection>());
        InstantiateNetworkArrowEgg(playerCount, conn);      
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

    public void SetPlayersConnection(List<PlayerConnection> newData)
    {
        _playerConnections = new List<PlayerConnection>(newData);
    }

    [Server]
    private void InstantiateNetworkArrowEgg(int index, NetworkConnection conn)
    {
        var playerArrow = Instantiate(spawnPrefabs[index]);
        NetworkServer.Spawn(playerArrow, conn);
        playerCount++;

        var networkArrow = playerArrow.GetComponent<NetworkArrowPosition>();
        networkArrow.CmdSetArrowPlayerConnection(PlayerConnections[playerCount - 1]);

        if (playerCount < 2)
            networkArrow.CmdUpdateArrowPosition(EventSystem.current.currentSelectedGameObject);
        else
        {
            PlayerSelectionUIController.instance.TargetGetServerPlayerCharacterSelectionData(conn, PlayerSelectionUIController.instance.CharacterButtons);
            var complementaryCharacterBtn = PlayerSelectionUIController.instance.CharacterButtons[1];
            PlayerConnections[0].PlayerSelectionData.NetworkArrow.CmdUpdateArrowPosition(PlayerSelectionUIController.instance.CharacterButtons[0]);
            networkArrow.CmdUpdateArrowPosition(complementaryCharacterBtn);
            PlayerSelectionUIController.instance.TargetGetServerPlayerConnectionsData(conn, PlayerConnections);
        }
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