using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Title("Player References")]
    private Player player;

    [Title("References")]
    public Transform cameraPosition;
    public Transform orientation;
    private GameCamera gameCamera;

    [Title("Rotation Control")]
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    private float xRotation;
    private float yRotation;

    public void Initialize(Player player)
    {
        this.player = player;

        StartCoroutine(WaitForGameCamera());
    }

    public void UpdateCamera()
    {
        MyInput();

        if (gameCamera != null)
        {
            gameCamera.UpdateCamera(xRotation, yRotation);
        }
    }

    private void MyInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * mouseSensX;
        xRotation -= mouseY * mouseSensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private IEnumerator WaitForGameCamera()
    {
        yield return new WaitUntil(() => GameCamera.Instance != null);
        gameCamera = GameCamera.Instance;
        gameCamera.Initialize(this);
    }
}