using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    private Camera mainCamera;

    private List<Unit> selectedUnits = new List<Unit>();

    [SerializeField]
    private LayerMask unitLayerMask = new LayerMask();
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DeselectAllUnits();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    public List<Unit> currentSelectedUnits()
    {
        return selectedUnits;
    }

    private void ClearSelectionArea()
    {
        Ray selectionRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(selectionRay, out RaycastHit selection, Mathf.Infinity, unitLayerMask))
        {
            SelectUnit(selection);
        }
    }

    private void SelectUnit(RaycastHit selection)
    {
        if(!selection.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if(!unit.hasAuthority) { return; }

        selectedUnits.Add(unit);

        foreach(Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }   
    }

    private void DeselectAllUnits()
    {
        foreach(Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Deselect();
        }
        selectedUnits.Clear();
    }
}
