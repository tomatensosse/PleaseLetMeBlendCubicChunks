using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public float tickRate = 3f;
    private float timeSinceLastTick;

    public override void Update()
    {
        base.Update();

        if (NetworkServer.active)
        {
            timeSinceLastTick += Time.deltaTime;

            if (timeSinceLastTick >= tickRate)
            {
                timeSinceLastTick = 0f;

                ServerTick();
            }
        }
    }

    [Server]
    private void ServerTick()
    {
        HandoutRandomItems();
    }

    private void HandoutRandomItems()
    {
        Debug.Log("Handing out random items to players...");

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null && conn.identity.TryGetComponent(out Player player))
            {
                // Example logic to give a random item to the player
                ItemInstance randomItem = ItemDatabase.Instance.GetRandomItem();
                player.Inventory.AddItem(randomItem);

                Debug.Log($"Gave {randomItem.Item.name} to player {player.netId}");
            }
        }
    }
}