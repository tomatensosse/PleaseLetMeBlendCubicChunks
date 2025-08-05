using Mirror;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    private Player player;

    public readonly SyncList<ItemInstance> inventory = new SyncList<ItemInstance>();

    public void Initialize(Player player)
    {
        this.player = player;

        if (isLocalPlayer)
        {
            inventory.OnChange += OnInventoryChanged;
        }
    }

    public void AddItem(ItemInstance itemInstance)
    {
        if (itemInstance.Item.maxStack == 0)
        {
            Debug.LogWarning($"Item {itemInstance.Item.name} has a max stack size of 0. Not adding to inventory to avoid crash.");
            return;
        }

        if ((itemInstance.Item.maxStack == 1 || !itemInstance.Item.isStackable) && itemInstance.amount == 1)
        {
            inventory.Add(itemInstance);
            return;
        }

        Item item = itemInstance.Item;
        int amountRemaining = itemInstance.amount;

        while (amountRemaining > 0)
        {
            int index = inventory.FindIndex(i => i.uid == item.uid && i.amount < item.maxStack);

            if (index >= 0)
            {
                ItemInstance existingItemInstance = inventory[index];

                int amountToAdd = Mathf.Min(item.maxStack - existingItemInstance.amount, amountRemaining);
                int newAmount = existingItemInstance.amount + amountToAdd;
                existingItemInstance.amount = newAmount;
                amountRemaining -= amountToAdd;

                inventory[index] = existingItemInstance; // Triggers OP_SET on Clients
            }
            else
            {
                int amountToAdd = Mathf.Min(item.maxStack, amountRemaining);
                amountRemaining -= amountToAdd;

                ItemInstance newItemInstance = new ItemInstance(item, amountToAdd);

                inventory.Add(newItemInstance); // Triggers OP_ADD on Clients
            }
        }
    }

    private void OnInventoryChanged(SyncList<ItemInstance>.Operation op, int index, ItemInstance itemInstance)
    {
        if (isLocalPlayer)
        {
            Debug.Log($"Inventory changed: {op} {inventory[index].Item.name} x{inventory[index].amount}");
        }
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            inventory.OnChange -= OnInventoryChanged;
        }
    }
}