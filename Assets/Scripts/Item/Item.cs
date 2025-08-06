using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string uid;
    public string itemName;
    public Sprite icon;
    public ItemCategory category;

    public bool isStackable;
    [ShowIf("isStackable"), Min(1)] public int maxStack = 1; // shouldnt be 1
    public bool isHoldable;
    [ShowIf("isHoldable")] public GameObject holdablePrefab;
}