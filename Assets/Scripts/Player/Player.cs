using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : Entity
{
    public static Player LocalPlayer { get; private set; }

    [Required] public PlayerMovement Movement;
    [Required] public PlayerCamera Camera;
    [Required] public PlayerInventory Inventory;

    protected override void ClientInitialize()
    {
        base.ClientInitialize();

        if (isLocalPlayer)
        {
            LocalPlayer = this;

            Movement.Initialize(this, rb);
            Camera.Initialize(this);
            Inventory.Initialize(this);
        }
    }

    protected override void LocalUpdate()
    {
        base.LocalUpdate();

        if (isOwned)
        {
            Movement.UpdateMovement();
            Camera.UpdateCamera();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryUI.Instance.ToggleInventory();
        }
    }

    protected override void LocalFixedUpdate()
    {
        base.LocalFixedUpdate();

        if (isOwned)
        {
            Movement.FixedUpdateMovement();
        }
    }
}
