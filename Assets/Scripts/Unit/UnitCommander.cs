using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;

    [SerializeField]
    private UnitSelectionHandler selectionHandler = null;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray godRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(godRay, out RaycastHit godTouch, Mathf.Infinity))
            {
                foreach(Unit selectedUnit in selectionHandler.currentSelectedUnits())
                {
                    if(selectedUnit)
                    {
                        selectedUnit.movementComponent().CmdMoveTo(godTouch.point);
                    }
                }
            }
        }
    }
}
