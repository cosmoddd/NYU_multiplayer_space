using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerGC : NetworkManager
{
    public static CharacterData dataMessage;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CharacterData>(OnCreateCharacter);
    }

    public override void Start()
    {
        base.Start();

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        
    }

    void OnCreateCharacter(NetworkConnection conn, CharacterData dataMessage)
    {
        GameObject character = Instantiate(playerPrefab);


    }
}
