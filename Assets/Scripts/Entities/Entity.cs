using System.Collections;
using System.Collections.Generic;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    protected bool isInitialized = false;

    [Required] public NetworkTransformReliable nt;
    [Required] public Rigidbody rb;

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerInitialize();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientInitialize();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorityInitialize();
    }

    protected virtual void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }

        if (isClient)
        {
            ClientUpdate();
        }

        LocalUpdate();
    }

    protected virtual void FixedUpdate()
    {
        if (isServer)
        {
            ServerFixedUpdate();
        }

        if (isClient)
        {
            ClientFixedUpdate();
        }

        LocalFixedUpdate();
    }

    protected virtual void ServerInitialize()
    {
        if (!isServer)
        {
            return;
        }

        isInitialized = true;
    }

    protected virtual void ClientInitialize()
    {
        // Client Initialize runs on both the server and client
    }

    protected virtual void AuthorityInitialize()
    {
        // Authority Initialize runs only on the client with authority
    }

    protected virtual void ServerUpdate()
    {
        // Server Update runs only on the server
    }

    protected virtual void ClientUpdate()
    {
        // Client Update runs only on the client
    }

    protected virtual void LocalUpdate()
    {
        // Local Update runs on both single player and multi player
    }

    protected virtual void ServerFixedUpdate()
    {
        // Server Fixed Update runs only on the server
    }

    protected virtual void ClientFixedUpdate()
    {
        // Client Fixed Update runs only on the client
    }

    protected virtual void LocalFixedUpdate()
    {
        // Local Fixed Update runs on both single player and multi player
    }
}
