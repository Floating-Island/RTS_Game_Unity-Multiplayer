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
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase @base)
    {
        if (@base.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        
        // copy list because whenever we destroy a unit, it will remove itself from the list, changing the iterator.
        List<Unit> remainingUnits = new List<Unit>();
        remainingUnits.AddRange(units);

        foreach(Unit unit in remainingUnits)
        {
            if (unit == null) { continue; }
            unit.ServerHandleDie();
        }
    }

    [Server]
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId == connectionToClient.connectionId)
        {
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
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
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
