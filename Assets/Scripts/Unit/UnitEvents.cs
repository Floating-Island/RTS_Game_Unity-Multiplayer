using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitEvents : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent onSelected = null;

    [SerializeField]
    private UnityEvent onDeselected = null;


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
