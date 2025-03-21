using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Controls controls;
    private Vector2 previousInput;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);
        SetupControls();
    }

    private void SetupControls()
    {
        controls = new Controls();

        // Unsubscribe is automatic.
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    private void SetPreviousInput(InputAction.CallbackContext context)
    {
        previousInput = context.ReadValue<Vector2>();
    }

    [ClientCallback]
    private void Update()
    {
        if (!netIdentity.isOwned) { return; }

        if (!Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 currentPosition = playerCameraTransform.position;

        float moveAcceleration = speed * Time.deltaTime;

        if (previousInput == Vector2.zero)
        {
            //currentPosition += UpdatePositionFromMouse() * moveAcceleration;
        }
        else
        {
            currentPosition += new Vector3(previousInput.x, 0f, previousInput.y) * moveAcceleration;
        }

        currentPosition.x = Mathf.Clamp(currentPosition.x, screenXLimits.x, screenXLimits.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = currentPosition;
    }

    private Vector3 UpdatePositionFromMouse()
    {
        // Check if the mouse moved towards one of the edges
        Vector3 cursorMovement = Vector3.zero;

        Vector2 cursorPosition = Mouse.current.position.ReadValue();

        if (cursorPosition.y >= Screen.height - screenBorderThickness)
        {
            cursorMovement.z += 1;
        }
        else if (cursorPosition.y <= screenBorderThickness)
        {
            cursorMovement.z -= 1;
        }

        if (cursorPosition.x >= Screen.height - screenBorderThickness)
        {
            cursorMovement.x += 1;
        }
        else if (cursorPosition.x <= screenBorderThickness)
        {
            cursorMovement.x -= 1;
        }

        // cursorMovement.normalized so it moves uniformly.
        return cursorMovement.normalized;
    }
}
