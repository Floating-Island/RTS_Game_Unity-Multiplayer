using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Networked_Player : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        units.Add(unit);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        units.Remove(unit);
    }
}
