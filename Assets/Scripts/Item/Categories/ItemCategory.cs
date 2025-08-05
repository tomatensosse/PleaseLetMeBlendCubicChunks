using UnityEngine;

[CreateAssetMenu(fileName = "NewItemCategory", menuName = "Inventory/ItemCategory")]
public class ItemCategory : ScriptableObject
{
    public string categoryName;
    public string categoryDisplayName;
    public string categoryDescription;
    public Sprite categoryIcon;
}