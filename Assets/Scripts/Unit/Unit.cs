using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;


    [SerializeField]
    private Pointed_Movement unitMovement = null;

    [SerializeField]
    private Targeter targeter = null;

    [SerializeField]
    private UnityEvent onSelected = null;

    [SerializeField]
    private UnityEvent onDeselected = null;

    [SerializeField]
    private Health health = null;

    public Pointed_Movement GetUnitMovement()
    {
        return unitMovement;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    [Client]
    public void Select()
    {
        if(hasAuthority)
        {
            onSelected?.Invoke();
        }
    }

    [Client]
    public void Deselect()
    {
        if(hasAuthority)
        {
            onDeselected?.Invoke();
        }
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }
}
