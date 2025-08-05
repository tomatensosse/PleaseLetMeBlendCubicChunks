using System;

[System.Serializable]
public struct ItemInstance
{
    public string guid;
    public string uid;

    public int amount;

    public Item Item => ItemDatabase.Instance.GetItem(uid);

    public ItemInstance(Item item, int amount = 1)
    {
        guid = Guid.NewGuid().ToString();

        this.uid = item.uid;
        this.amount = amount;
    }
}