using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UtopiaUI
{
    public static InventoryUI Instance { get; private set; }

    public RectTransform categoryContainer;
    public VerticalLayoutGroup verticalLayoutGroup;

    public GameObject categoryUIPrefab;
    public GameObject itemUIPrefab;

    private Dictionary<ItemCategory, CategoryUI> categoryUIs = new Dictionary<ItemCategory, CategoryUI>();
    private List<ItemUI> items = new List<ItemUI>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(ItemInstance itemInstance, int index)
    {
        if (categoryUIs.TryGetValue(itemInstance.Item.category, out CategoryUI categoryUI))
        {
            ItemUI newItemUI = CreateItemUI(itemInstance, index, categoryUI);
            items.Add(newItemUI);
        }
        else
        {
            CategoryUI newCategoryUI = Instantiate(categoryUIPrefab, categoryContainer).GetComponent<CategoryUI>();
            categoryUIs[itemInstance.Item.category] = newCategoryUI;
            newCategoryUI.Initialize(itemInstance.Item.category);

            ItemUI newItemUI = CreateItemUI(itemInstance, index, newCategoryUI);
            items.Add(newItemUI);
        }

        StartCoroutine(DelayedLayoutFix());
    }

    public void UpdateItem(ItemInstance itemInstance, int index)
    {
        if (items.Find(i => i.Index == index) is ItemUI itemUI)
        {
            itemUI.Set(itemInstance, index);
        }

        StartCoroutine(DelayedLayoutFix());
    }

    private ItemUI CreateItemUI(ItemInstance itemInstance, int index, CategoryUI categoryUI)
    {
        ItemUI itemUI = Instantiate(itemUIPrefab, categoryUI.itemContainer).GetComponent<ItemUI>();
        itemUI.Set(itemInstance, index);
        items.Add(itemUI);
        return itemUI;
    }

    IEnumerator DelayedLayoutFix()
    {
        yield return null; // Wait 1 frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(categoryContainer);
    }
}
