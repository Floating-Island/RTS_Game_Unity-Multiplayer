using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Networked_Player : NetworkBehaviour
{
    [SerializeField] private BuildingFactory buildingFactory = null;
    [SerializeField] private ResourceStorage resourceStorage = null;
    private List<Unit> units = new List<Unit>();
    
    [SerializeField]
    private List<Building> buildings = new List<Building>();

    public List<Unit> GetUnits()
    {
        return units;
    }

    public List<Building> GetBuildings()
    {
        return buildings;
    }

    public BuildingFactory GetBuildingFactory()
    {
        return buildingFactory;
    }

    public ResourceStorage GetResourceStorage()
    {
        return resourceStorage;
    }

    [Client]
    public static RTS_Networked_Player ClientNetworkedPlayer()
    {
        return NetworkClient.connection.identity.GetComponent<RTS_Networked_Player>();
    }

    [Server]
    public static RTS_Networked_Player ServerNetworkedPlayer(NetworkBehaviour gameObject)
    {
        return gameObject.connectionToClient.identity.GetComponent<RTS_Networked_Player>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if(building.connectionToClient.connectionId == connectionToClient.connectionId)
        {
            if (buildings.Contains(building)) { return; }
            buildings.Add(building);
        }
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if(building.connectionToClient.connectionId == connectionToClient.connectionId)
        {
            buildings.Remove(building);
        }
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 spawnLocation)
    {
        buildingFactory.ServerSpawnBuilding(buildingId, spawnLocation);
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
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
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

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        buildings.Remove(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        buildings.Add(building);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
}
