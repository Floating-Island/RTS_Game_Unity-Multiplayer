using System;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    [SerializeField]
    private Health health = null;

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        ServerOnBaseDespawned?.Invoke(this);
    }
}
