using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject unitPrefab = null;

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, transform.position, transform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(hasAuthority && eventData.button == PointerEventData.InputButton.Right)
        {
            CmdSpawnUnit();
        }
    }
}
