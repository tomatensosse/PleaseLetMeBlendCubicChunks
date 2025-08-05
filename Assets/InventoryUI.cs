using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    public Transform inventoryUI;

    private Dictionary<ItemCategory, CategoryUI> categoryUIs = new Dictionary<ItemCategory, CategoryUI>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ToggleInventory()
    {
        inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
    }
}
