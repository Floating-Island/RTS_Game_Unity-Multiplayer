using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    private Camera mainCamera;

    private Unit selectedUnit = null;

    [SerializeField]
    private LayerMask unitLayerMask = new LayerMask();
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Ray selectionRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(selectionRay, out RaycastHit selection, Mathf.Infinity, unitLayerMask))
            {
                if(selection.collider.TryGetComponent<Unit>(out Unit unit))
                {
                    selectedUnit?.Deselect();

                    selectedUnit = unit;

                    selectedUnit.Select();
                }
            }
        }
    }

    public Unit currentSelectedUnit()
    {
        return selectedUnit;
    }
}
