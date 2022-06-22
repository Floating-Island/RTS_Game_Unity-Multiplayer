using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_NetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject unitSpawnerPrefab = null;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Transform playerTransform = conn.identity.transform;
        GameObject unitSpawner = Instantiate(unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
        NetworkServer.Spawn(unitSpawner, conn);
    }
}
