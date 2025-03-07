using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    private Camera mainCamera = null;

    [SerializeField]
    private UnitSelectionHandler selectionHandler = null;

    [SerializeField]
    private LayerMask unitLayerMask = new LayerMask();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(!Mouse.current.leftButton.wasPressedThisFrame) { return; }
        
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask))
        {
            TryMove(hit.point);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit selectedUnit in selectionHandler.currentSelectedUnits())
        {
            if(selectedUnit)
            {
                selectedUnit.movementComponent().CmdMoveTo(point);
            }
        }
    }
}
