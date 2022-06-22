using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject unitPrefab = null;

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, transform.position, transform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }
}
