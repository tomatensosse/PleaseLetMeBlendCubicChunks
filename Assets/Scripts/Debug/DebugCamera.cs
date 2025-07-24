using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    public static DebugCamera Instance { get; private set; }

    public float sensitivity = 3f;
    public float smoothSpeed = 0.1f;

    public Transform anchor;

    protected float distance = 15f;

    private float targetYaw;
    private float targetPitch;
    private float yaw;
    private float pitch;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void LateUpdate()
    {
        if (anchor == null) { anchor = transform; }

        // scroll wheel distance +-
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            distance -= scroll * 5f;
            distance = Mathf.Clamp(distance, 10f, 30f);
        }

        // Only rotate when holding right mouse button
        if (Input.GetMouseButton(1))
        {
            targetYaw += Input.GetAxis("Mouse X") * sensitivity;
            targetPitch -= Input.GetAxis("Mouse Y") * sensitivity;
            targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
        }

        // Smooth yaw and pitch
        yaw = Mathf.Lerp(yaw, targetYaw, Time.deltaTime * smoothSpeed);
        pitch = Mathf.Lerp(pitch, targetPitch, Time.deltaTime * smoothSpeed);

        // Build rotation and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = anchor.position + offset;
        transform.rotation = rotation;
    }
}