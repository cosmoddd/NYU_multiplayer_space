using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerGC : NetworkManager
{
    public CharacterData dataMessage = new CharacterData();

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CharacterData>(OnCreateCharacter);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        conn.Send(dataMessage);
    }

    void OnCreateCharacter(NetworkConnection conn, CharacterData dataMessage)
    {
        // TODO check for selected character type to determine whether
        // to use CustomCharacter prefab or use other Character prefab
        GameObject character = Instantiate(playerPrefab);
        

        NetworkServer.AddPlayerForConnection(conn, character);
    }

    public void DisplayData()
    {
        foreach(int ID in dataMessage.bodyIDs)
        {
            Debug.Log(ID);
        }

        foreach (Vector3 color in dataMessage.bodyColors)
        {
            Debug.Log(color);
        }

        Debug.Log(dataMessage.userName);
    }
}
