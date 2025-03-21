using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;

public class RTS_Networked_Player : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private BuildingFactory buildingFactory = null;
    [SerializeField] private ResourceStorage resourceStorage = null;
    private List<Unit> units = new List<Unit>();
    
    [SerializeField]
    private List<Building> buildings = new List<Building>();

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool partyOwner = false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    private Color teamColor = new Color();

    public static event Action ClientOnInfoUpdated;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

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

    public bool IsPartyOwner()
    {
        return partyOwner;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    [Server]
    public void SetDisplayName(string newName)
    {
        displayName = newName;
    }

    [Server]
    public void SetAsPartyOwner()
    {
        partyOwner = true;
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

        DontDestroyOnLoad(gameObject);
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

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public Color GetTeamColor()
    {
        return teamColor;
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
        }
    }

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        units.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        units.Remove(unit);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        buildings.Remove(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        buildings.Add(building);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!netIdentity.isOwned) { return; }
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    [Command]
    public void CmdStartGame()
    {
        if (!partyOwner) { return; }

        RTS_NetworkManager networkManager = (RTS_NetworkManager)NetworkManager.singleton;
        networkManager.StartGame();
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        RTS_NetworkManager networkManager = (RTS_NetworkManager)NetworkManager.singleton;
        networkManager.AddPlayerToList(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }

        RTS_NetworkManager networkManager = (RTS_NetworkManager)NetworkManager.singleton;
        networkManager.RemovePlayerFromList(this);

        if(!netIdentity.isOwned) { return; }

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
}
