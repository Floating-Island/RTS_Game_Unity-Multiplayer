using System;
using System.Security.Cryptography;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField]
    private int id = -1;

    [SerializeField]
    private Sprite icon = null;

    [SerializeField]
    private int price = 100;

    [SerializeField]
    private GameObject buildingPreview = null;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetPrice()
    {
        return price;
    }

    public int GetId()
    {
        return id;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    public bool CollidesAtLocation(Vector3 location, LayerMask buildingLayerMask)
    {
        bool collides = false;
        BoxCollider buildingCollider = GetComponent<BoxCollider>();
        Vector3 colliderExtent = buildingCollider.size / 2;
        if (Physics.CheckBox(location + buildingCollider.center, colliderExtent, transform.rotation, buildingLayerMask))
        {
            collides = true;
        }
        return collides;
    }
}
