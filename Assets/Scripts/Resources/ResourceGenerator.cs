using System;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float intervalSeconds = 2f;

    private RTS_Networked_Player player;

    public override void OnStartServer()
    {
        player = connectionToClient.identity.GetComponent<RTS_Networked_Player>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;

        InvokeRepeating(nameof(AddResources), intervalSeconds, intervalSeconds);
    }

    private void AddResources()
    {
        player.GetResourceStorage().ServerAddResource(resourcesPerInterval);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        StopAddingResources();
    }

    private void StopAddingResources()
    {
        CancelInvoke(nameof(AddResources));
    }

    private void ServerHandleGameOver(int winnerConnectionId)
    {
        StopAddingResources();
        enabled = false;
    }

    private void ServerHandleDie()
    {
        StopAddingResources();
        NetworkServer.Destroy(gameObject);
    }
}
