using System;
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
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask)) { return; }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.CanBeTargeted())
            {
                TryTargetMove(target);
                return;
            }
        }
        TryMove(hit.point);
    }

    private void TryTargetMove(Targetable target)
    {
        foreach(Unit selectedUnit in selectionHandler.currentSelectedUnits())
        {
            if(selectedUnit)
            {
                selectedUnit.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit selectedUnit in selectionHandler.currentSelectedUnits())
        {
            if(selectedUnit)
            {
                selectedUnit.GetUnitMovement().CmdMoveTo(point);
            }
        }
    }
}
