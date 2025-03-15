using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuildingFactory : NetworkBehaviour
{
    [SerializeField] List<Building> buildingPrefabs = new List<Building>();

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
        if (buildingPrefab == null) { return; }
        
        GameObject buildingInstance = Instantiate(buildingPrefab.gameObject, spawnLocation, new Quaternion());
        Debug.Log("Creating " + buildingInstance.name + " at location " + buildingInstance.transform.position);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }
}
