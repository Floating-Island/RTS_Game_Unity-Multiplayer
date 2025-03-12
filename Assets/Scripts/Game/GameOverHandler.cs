using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> bases = new List<UnitBase>();
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += HandleServerBaseSpawned;
        UnitBase.ServerOnBaseDespawned += HandleServerBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= HandleServerBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= HandleServerBaseDespawned;
    }

     [Server]
    private void HandleServerBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void HandleServerBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        CheckForGameOver();
    }

    [Server]
    private void CheckForGameOver()
    {
        if(bases.Count != 1) { return; }

        RpcGameOver();
    }

    [ClientRpc]
    private void RpcGameOver()
    {
        Debug.Log("Game Over. " + (connectionToClient.identity.GetComponent<RTS_Networked_Player>().GetUnits().Count > 0 ? "You Win!" : "You Lose!"));
    }
}
