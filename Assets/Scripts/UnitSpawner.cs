using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject unitPrefab = null;
    private GameObject spawnPoint = null;

    [Command]
    private void CmdSpawnUnit()
    {
        Transform spawnPointTransform = spawnPoint.transform;
        GameObject unitInstance = Instantiate(unitPrefab, spawnPointTransform.position, spawnPointTransform.rotation);

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
