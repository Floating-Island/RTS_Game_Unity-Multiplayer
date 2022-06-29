using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitEvents : NetworkBehaviour
{
    [SerializeField]
    private Pointed_Movement unitMovement = null;

    [SerializeField]
    private UnityEvent onSelected = null;

    [SerializeField]
    private UnityEvent onDeselected = null;

    public Pointed_Movement movementComponent()
    {
        return unitMovement;
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
}
