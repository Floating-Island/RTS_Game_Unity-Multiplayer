using System;
using System.Threading;
using Mirror;
using UnityEngine;

public class ResourceStorage : NetworkBehaviour
{
    [SerializeField]
    private int startResourceAmount = 500;

    [SyncVar(hook = nameof(ResourceUpdated))]
    private int resource = 0;

    public event Action<int> ClientHandleResourcesUpdated;

    public int CurrentResourceAmount()
    {
        return resource;
    }

    private void ResourceUpdated(int oldResource, int newResource)
    {
        ClientHandleResourcesUpdated?.Invoke(newResource);
    }

    public override void OnStartServer()
    {
        if (resource == 0)
        {
            ServerAddResource(startResourceAmount);
        }
    }

    [Server]
    public void ServerAddResource(int amount)
    {
        // Interlocked.Add doesn't work with sync vars...
        lock(this)
        {
            resource += amount;
        }
    }

    [Server]
    public bool ServerAttemptRemoveResource(int amount)
    {
        bool RemovalSuccessful = false;
        
        lock(this)
        {
            if (resource >= amount)
            {
                resource -= amount;
                RemovalSuccessful = true;
            }
        }

        return RemovalSuccessful;
    }
}
