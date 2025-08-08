using UnityEngine;

public class UtopiaDebugger : MonoBehaviour
{
    public static UtopiaDebugger Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}