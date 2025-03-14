using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Networked_Player : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();

    public List<Unit> GetUnits()
    {
        return units;
    }

    public static RTS_Networked_Player NetworkedPlayer()
    {
        return NetworkClient.connection.identity.GetComponent<RTS_Networked_Player>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    [Server]
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId == connectionToClient.connectionId)
        {
            if (units.Contains(unit)) { return; }
            units.Add(unit);
            AddToClientSide(unit);
        }
    }

    [ClientRpc]
    private void AddToClientSide(Unit aUnit)
    {
        if(hasAuthority && isClientOnly)
        {
            units.Add(aUnit);
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    [Server]
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId == connectionToClient.connectionId)
        {
            units.Remove(unit);
            RemoveFromClientSide(unit);
        }
    }

    [ClientRpc]
    private void RemoveFromClientSide(Unit aUnit)
    {
        if(hasAuthority && isClientOnly)
        {
            units.Remove(aUnit);
        }
    }
}
