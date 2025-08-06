using UnityEngine;

public class UtopiaCanvas : MonoBehaviour
{
    public static UtopiaCanvas Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    
}