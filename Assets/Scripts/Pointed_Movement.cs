using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Pointed_Movement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent = null;

    [SerializeField]
    private Camera mainCamera = null;

    [Command]
    public void CmdMoveTo(Vector3 aPosition)
    {
        float maxNavMeshDistance = 1f;

        if(NavMesh.SamplePosition(aPosition, out NavMeshHit aHit, maxNavMeshDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(aHit.position);
        }
    }
 
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if(hasAuthority && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray godRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(godRay, out RaycastHit godTouch, Mathf.Infinity))
            {
                CmdMoveTo(godTouch.point);
            }
        }
    }
}
