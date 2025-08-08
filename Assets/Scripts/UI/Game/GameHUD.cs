using UnityEngine;

public class GameHUD : UtopiaUI
{
    public override void OnEnable()
    {
        base.OnEnable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}