using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField]
    private Unit unitPrefab = null;
    
    [SerializeField]
    private GameObject spawnPoint = null;

    [SerializeField]
    private Health health = null;

    
    [SerializeField]
    private float spawnMoveRange = 7f;

    
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void SpawnUnit()
    {
        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnPoint.transform.position.y;

        Transform spawnPointTransform = spawnPoint.transform;
        spawnPointTransform.position += spawnOffset;

        GameObject unitInstance = Instantiate(unitPrefab.gameObject, spawnPointTransform.position, spawnPointTransform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    [Server]
    public bool BuyUnit()
    {
        RTS_Networked_Player player = RTS_Networked_Player.ServerNetworkedPlayer(this);

        int unitPrice = unitPrefab.GetCost();
        return player.GetResourceStorage().ServerAttemptRemoveResource(unitPrice);
    }
}
