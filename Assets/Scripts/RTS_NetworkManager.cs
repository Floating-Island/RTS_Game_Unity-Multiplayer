using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTS_NetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject unitSpawnerPrefab = null;
    
    [SerializeField]
    private GameOverHandler gameOverHandler = null;

    [SerializeField]
    private TeamColor teamColors;

    private List<RTS_Networked_Player> players = new List<RTS_Networked_Player>();

    private bool gameInProgress = false;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<RTS_Networked_Player> GetPlayers()
    {
        return players;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (gameInProgress)
        {
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        RTS_Networked_Player player = conn.identity.GetComponent<RTS_Networked_Player>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        players.Clear();

        gameInProgress = false;
    }

    public override void OnStopClient()
    {
        players.Clear();
    }

    public void StartGame()
    {
        if (players.Count < 2) { return; }

        gameInProgress = true;

        ServerChangeScene("Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        RTS_Networked_Player player = conn.identity.GetComponent<RTS_Networked_Player>();
        player.SetTeamColor(teamColors.GetColor());

        players.Add(player);
        player.SetDisplayName($"Player {players.Count}");

        if (players.Count == 1)
        {
            player.SetAsPartyOwner();
        }
    }

    private void SpawnPlayerBase(RTS_Networked_Player player)
    {
        GameObject baseInstance = Instantiate(unitSpawnerPrefab, GetStartPosition().position, Quaternion.identity);
        NetworkServer.Spawn(baseInstance, player.connectionToClient);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (RTS_Networked_Player player in players)
            {
                SpawnPlayerBase(player);
            }
        }
    }

    internal void RemovePlayerFromList(RTS_Networked_Player player)
    {
        players.Remove(player);
    }

    internal void AddPlayerToList(RTS_Networked_Player player)
    {
        players.Add(player);
    }
}
