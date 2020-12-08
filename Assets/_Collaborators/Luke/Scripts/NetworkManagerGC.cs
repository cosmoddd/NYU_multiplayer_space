using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class NetworkManagerGC : NetworkManager
{
    public static event Action<string> UserAdded;
    public CustomizerData dataMessage = new CustomizerData();

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Subscribes messages for adding players to this handler.  
        // Any time a player joins, the handler, OnCreateCharacter, will be called.
        NetworkServer.RegisterHandler<CustomizerData>(OnCreateCharacter);
    }

    public void SetServerAddress(string s)
    {
        networkAddress = s;
    }

    // when a client connects.  duh
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        // client sends message to server that it is here and ready to rumble
        conn.Send(dataMessage);
    }

    void OnCreateCharacter(NetworkConnection conn, CustomizerData dataMessage)
    {
        // TODO check for selected character type to determine whether
        // to use CustomCharacter prefab or use other Character prefab
        GameObject character = Instantiate(playerPrefab, GetStartPosition().position, GetStartPosition().rotation);

        MeshAssigner assigner = character.GetComponent<MeshAssigner>();
        if (assigner)
            assigner.LoadData(dataMessage);
        else
            Debug.Log("Could not find assigner");

        print($"{dataMessage.userName} is here and ready to rumble.");
        UserAdded?.Invoke(dataMessage.userName);
        NetworkServer.AddPlayerForConnection(conn, character);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        print("did someone show up to the party?");
    }


}
