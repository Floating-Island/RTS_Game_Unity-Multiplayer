using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCameraTransform = Camera.main.transform;    
    }

    // Update after camera moves
    void LateUpdate()
    {
        Quaternion mainCameraRotation = mainCameraTransform.rotation;
        Vector3 cameraForwardRotation = mainCameraRotation * Vector3.forward;
        Vector3 cameraUpRotation = mainCameraRotation * Vector3.up;

        transform.LookAt(transform.position + cameraForwardRotation, cameraUpRotation);
    }
}
