using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_NetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject unitSpawnerPrefab = null;
    
    [SerializeField]
    private GameOverHandler gameOverHandler = null;

    [SerializeField]
    private TeamColor teamColors;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        RTS_Networked_Player player = conn.identity.GetComponent<RTS_Networked_Player>();
        player.SetTeamColor(teamColors.GetColor());

        Transform playerTransform = conn.identity.transform;
        GameObject unitSpawner = Instantiate(unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
        NetworkServer.Spawn(unitSpawner, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (NetworkServer.active)
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
