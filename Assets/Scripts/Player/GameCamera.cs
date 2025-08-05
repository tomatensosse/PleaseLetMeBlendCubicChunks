using Sirenix.OdinInspector;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance { get; private set; }

    [Title("References")]
    public Transform cameraContainer;

    [Title("Target References", "References to the player's camera position and orientation.")]
    private Transform targetCameraPosition;
    private Transform targetOrientation;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(PlayerCamera playerCamera)
    {
        targetCameraPosition = playerCamera.cameraPosition;
        targetOrientation = playerCamera.orientation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateCamera(float xRotation, float yRotation)
    {
        cameraContainer.position = targetCameraPosition.position;

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        cameraContainer.localRotation = Quaternion.Euler(0, yRotation, 0);

        targetOrientation.rotation = cameraContainer.rotation;
    }
}