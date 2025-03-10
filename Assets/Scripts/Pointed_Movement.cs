using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pointed_Movement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent = null;

    [SerializeField]
    private Targeter targeter = null;
    
    [ServerCallback]
    private void Update()
    {
        if(!agent) { return; }

        if(!agent.hasPath) { return; }

        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    [Command]
    public void CmdMoveTo(Vector3 aPosition)
    {
        targeter.CmdClearTarget();

        float maxNavMeshDistance = 1f;

        if(NavMesh.SamplePosition(aPosition, out NavMeshHit aHit, maxNavMeshDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(aHit.position);
        }
    }
}
