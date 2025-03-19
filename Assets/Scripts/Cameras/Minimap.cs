using System;
using System.Drawing.Text;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRectangle = null;

    ///<summary>
    /// Half of the size of the floor.
    ///</summary>
    [SerializeField] private float mapScale = 20f;
    [SerializeField] private float cameraOffset = -6f;

    private Transform playerCameraTransform;

    private void Update()
    {
        SetupPlayerCameraTransform();
    }

    private void SetupPlayerCameraTransform()
    {
        if (playerCameraTransform != null) { return; }

        if (NetworkClient.connection.identity == null) { return; }

        playerCameraTransform = RTS_Networked_Player.ClientNetworkedPlayer().GetCameraTransform();
    }

    private void MoveCamera()
    {

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        bool mouseInMinimap = RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRectangle, mousePosition, null, out Vector2 localPoint);

        if (!mouseInMinimap) { return; }

        float minimapXPosition = (localPoint.x - minimapRectangle.rect.x) / minimapRectangle.rect.width;
        float minimapYPosition = (localPoint.y - minimapRectangle.rect.y) / minimapRectangle.rect.height;

        Vector2 minimapLocalPosition = new Vector2(minimapXPosition, minimapYPosition);

        float newCameraPositionX = Mathf.Lerp(-mapScale, mapScale, minimapXPosition);
        float newCameraPositionZ = Mathf.Lerp(-mapScale, mapScale, minimapYPosition);
        Vector3 newCameraPosition = new Vector3(newCameraPositionX, playerCameraTransform.position.y, newCameraPositionZ);

        playerCameraTransform.position = newCameraPosition + new Vector3(0,0, cameraOffset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
