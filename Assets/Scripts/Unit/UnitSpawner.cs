using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject unitPrefab = null;
    
    [SerializeField]
    private GameObject spawnPoint = null;

    [SerializeField]
    private Health health = null;
    
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

    [Command]
    private void CmdSpawnUnit()
    {
        Transform spawnPointTransform = spawnPoint.transform;
        GameObject unitInstance = Instantiate(unitPrefab, spawnPointTransform.position, spawnPointTransform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(hasAuthority && eventData.button == PointerEventData.InputButton.Right)
        {
            CmdSpawnUnit();
        }
    }
}
