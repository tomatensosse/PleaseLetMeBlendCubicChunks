using System;
using System.Collections.Generic;
using UnityEngine;

public class UtopiaCanvas : MonoBehaviour
{
    public static UtopiaCanvas Instance { get; private set; }
    public static bool IsInitialized => Instance != null && Instance.isInitialized;

    public Dictionary<Type, GameObject> instantiatedUIs = new Dictionary<Type, GameObject>();

    public Type latestUIType;

    private bool isInitialized = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public virtual void OnCanvasLoad()
    {
        isInitialized = true;
    }

    public virtual void EnableUI(GameObject gameObject)
    {
        
    }

    public virtual void EnableInitializedUI(Type)
    {
        foreach (var ui in instantiatedUIs.Values)
        {
            ui.SetActive(false);
        }

        instantiatedUI.SetActive(true);
        latestUIType = instantiatedUI.GetComponent<UtopiaUI>().GetType();
    }

    public virtual void ToggleInitializedUI(GameObject instantiatedUI)
    {
        if (latestUIType == instantiatedUI.GetComponent<UtopiaUI>().GetType())
        {
            instantiatedUI.SetActive(!instantiatedUI.activeSelf);
        }
        else
        {
            EnableInitializedUI(instantiatedUI);
        }
    }
}