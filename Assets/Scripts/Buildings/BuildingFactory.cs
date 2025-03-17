using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuildingFactory : NetworkBehaviour
{
    [SerializeField] List<Building> buildingPrefabs = new List<Building>();

    [SerializeField] float buildingRangeLimit = 4f;

    [SerializeField] LayerMask buildingLayerMask = new LayerMask();

    private Building FindBuilding(int buildingId)
    {
        Building buildingFound = null;
        foreach (Building building in buildingPrefabs)
        {
            if (building.GetId() == buildingId)
            {
                buildingFound = building;
                break;
            }
        }
        return buildingFound;
    }

    [Server]
    public void ServerSpawnBuilding(int buildingId, Vector3 spawnLocation)
    {
        Building buildingPrefab = FindBuilding(buildingId);

        if (!CanPlaceBuilding(spawnLocation, buildingPrefab)) { return; }
        Debug.Log("can place building");
        if (!BuyBuilding(buildingPrefab)) { return; }

        GameObject buildingInstance = Instantiate(buildingPrefab.gameObject, spawnLocation, buildingPrefab.transform.rotation);
        Debug.Log("Creating " + buildingInstance.name + " at location " + buildingInstance.transform.position);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    public bool CanPlaceBuilding(Vector3 location, Building buildingPrefab)
    {
        if (buildingPrefab == null) { Debug.Log("prefab is null"); return false; }

        if (BuildingCollides(location, buildingPrefab)) { Debug.Log("building collides"); return false; }

        if (!BuildingInRange(location)) { Debug.Log("building is not in range"); return false; }

        return true;
    }

    private bool BuildingInRange(Vector3 location)
    {
        bool inRange = false;

        RTS_Networked_Player player = null;
        // used by client and server so it needs to resolve which player to call.
        if (isServer)
        {
            player = RTS_Networked_Player.ServerNetworkedPlayer(this);
        }
        else
        {
            player = RTS_Networked_Player.ClientNetworkedPlayer();
        }
        
        float buildingSquareRangeLimit = buildingRangeLimit * buildingRangeLimit;

        foreach( Building building in player.GetBuildings())
        {
            if ((location - building.transform.position).sqrMagnitude <= buildingSquareRangeLimit)
            {
                inRange = true;
                break;
            }
        }
        return inRange;
    }

    private bool BuildingCollides(Vector3 spawnLocation, Building buildingPrefab)
    {
        return buildingPrefab.CollidesAtLocation(spawnLocation, buildingLayerMask);
    }

    [Server]
    private bool BuyBuilding(Building buildingPrefab)
    {
        RTS_Networked_Player player = RTS_Networked_Player.ServerNetworkedPlayer(this);

        int buildingPrice = buildingPrefab.GetPrice();
        return player.GetResourceStorage().ServerAttemptRemoveResource(buildingPrice);
    }
}
