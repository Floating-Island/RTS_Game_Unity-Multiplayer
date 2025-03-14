using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    private Camera mainCamera;

    private List<Unit> selectedUnits = new List<Unit>();

    [SerializeField] private LayerMask unitLayerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Vector2 selectionStartPosition = Vector2.zero;
    private RTS_Networked_Player player;
    
    private void Start()
    {
        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += DeselectUnit;
        GameOverHandler.ClientPlayerDied += HandleClientOnPlayerDied;
        GameOverHandler.ClientOnGameOver += HandleClientOnPlayerDied;
    }

    private void HandleClientOnPlayerDied(int connectionId)
    {
        if (connectionId != player.connectionToClient.connectionId) { return; }

        gameObject.SetActive(false);
    }

    private void DeselectUnit(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= DeselectUnit;
        GameOverHandler.ClientPlayerDied -= HandleClientOnPlayerDied;
        GameOverHandler.ClientOnGameOver -= HandleClientOnPlayerDied;
    }

    private void StoreNetworkedPlayer()
    {
        if (player == null)
        {
            player = RTS_Networked_Player.NetworkedPlayer();
        }
    }

    private void Update()
    {
        StoreNetworkedPlayer();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HideSelectionArea();
            CommandUnits();
        }
        else if(Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - selectionStartPosition.x;
        float areaHeight = mousePosition.y - selectionStartPosition.y;
        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = selectionStartPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    public List<Unit> currentSelectedUnits()
    {
        return selectedUnits;
    }

    private void CommandUnits()
    {
        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            SelectSingle();
        }
        else
        {
            SelectMultiple();
        }
    }

    private void HideSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);
    }

    private void SelectMultiple()
    {
        foreach(Unit unit in player.GetUnits())
        {
            if(selectedUnits.Contains(unit)) { continue; }

            if(UnitInsideSelectionArea(unit))
            {
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private bool UnitInsideSelectionArea(Unit unit)
    {
        Vector2 unitScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
        // unitSelectionArea.rect is in local space, so we need to subtract the anchoredPosition.
        return unitSelectionArea.rect.Contains(unitScreenPosition - unitSelectionArea.anchoredPosition, true);
    }

    private void SelectSingle()
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

    private void StartSelectionArea()
    {
        if (!AddToMultipleSelection())
        {
            DeselectAllUnits();
        }

        selectionStartPosition = Mouse.current.position.ReadValue();
        ShowSelectionArea();

        UpdateSelectionArea();
    }

    private static bool AddToMultipleSelection()
    {
        return Keyboard.current.leftShiftKey.isPressed;
    }

    private void ShowSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(true);
    }

    private void DeselectAllUnits()
    {
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Deselect();
        }
        selectedUnits.Clear();
    }
}
