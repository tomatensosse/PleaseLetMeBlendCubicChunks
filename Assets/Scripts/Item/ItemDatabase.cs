using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public static bool IsInitialized => Instance != null && Instance.isInitialized;

    private bool isInitialized = false;
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        PopulateDatabase();
    }

    private void PopulateDatabase()
    {
        Item[] items = Resources.LoadAll<Item>("Items");

        foreach (Item item in items)
        {
            itemDictionary.Add(item.uid, item);
        }

        Debug.Log($"{itemDictionary.Count} items loaded into database.");
    }

    public Item GetItem(string uid)
    {
        if (itemDictionary.ContainsKey(uid))
        {
            return itemDictionary[uid];
        }

        Debug.LogError($"Item with UID {uid} not found in database.");

        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("Item UID is null or empty.");
        }

        return null;
    }

    public ItemInstance GetRandomItem()
    {
        if (itemDictionary.Count == 0)
        {
            Debug.LogError("Item database is empty. Cannot get a random item.");
            return default;
        }

        int randomIndex = Random.Range(0, itemDictionary.Count);
        string randomUid = new List<string>(itemDictionary.Keys)[randomIndex];
        Item randomItem = GetItem(randomUid);

        int randomAmount = Random.Range(1, randomItem.maxStack);

        if (randomItem != null)
        {
            return new ItemInstance(randomItem, randomAmount);
        }

        Debug.LogError("Failed to create ItemInstance for random item.");
        return default;
    }
}