using UnityEngine;

public abstract class UtopiaUI : MonoBehaviour
{
    public virtual void OnEnable()
    {
        Debug.Log($"Enabled: {GetType().Name}");
    }

    public virtual void OnDisable()
    {
        Debug.Log($"Disabled: {GetType().Name}");
    }
}