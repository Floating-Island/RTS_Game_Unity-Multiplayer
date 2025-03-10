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

    [SerializeField]
    private float chaseRange = 10f;
    
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.CurrentTarget();
        if(target == null)
        {
            if(!agent) { return; }

            if(!agent.hasPath) { return; }

            if(agent.remainingDistance > agent.stoppingDistance) { return; }

            agent.ResetPath();
        }
        else
        {
            float targetDistance = (target.transform.position - transform.position).sqrMagnitude;
            if(targetDistance > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if(agent.hasPath)
            {
                agent.ResetPath();
            }
        }
    }

    [Command]
    public void CmdMoveTo(Vector3 aPosition)
    {
        targeter.ClearTarget();

        float maxNavMeshDistance = 1f;

        if(NavMesh.SamplePosition(aPosition, out NavMeshHit aHit, maxNavMeshDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(aHit.position);
        }
    }
}
