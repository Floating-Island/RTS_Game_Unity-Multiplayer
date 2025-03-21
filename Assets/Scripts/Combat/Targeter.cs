using System;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public override void OnStartServer()
    {
        GameOverHandler.ServerPlayerDied += ServerHandlePlayerDied;
    }

    private void ServerHandlePlayerDied(int connectionId)
    {
        if (target != null && connectionId == target.connectionToClient.connectionId)
        {
            ClearTarget();
        }
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerPlayerDied -= ServerHandlePlayerDied;
    }

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if (!target.TryGetComponent<Targetable>(out Targetable targetable))
        { 
            return;
        }

        this.target = targetable;
    }

    [Server]
    public void ClearTarget()
    {
        if(netIdentity.isOwned)
        {
            target = null;
        }
    }

    public Targetable CurrentTarget()
    {
        return target;
    }
}
