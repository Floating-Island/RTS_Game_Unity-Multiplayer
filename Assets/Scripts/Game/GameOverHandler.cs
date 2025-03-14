using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<int> ClientOnGameOver;
    public static event Action<int> ServerOnGameOver;
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

        int winnerId = bases[0].connectionToClient.connectionId;
        RpcGameOver(winnerId);
        ServerOnGameOver?.Invoke(winnerId);
    }

    [ClientRpc]
    private void RpcGameOver(int winnerId)
    {
        ClientOnGameOver?.Invoke(winnerId);
    }
}
