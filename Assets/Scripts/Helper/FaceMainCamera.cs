using UnityEngine;

public class FaceMainCamera : MonoBehaviour
{
    protected Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Make this object face the main camera
            transform.LookAt(mainCamera.transform);
            // Optionally, you can also adjust the rotation to keep the upright orientation
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}