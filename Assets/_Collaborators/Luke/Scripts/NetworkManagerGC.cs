using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerGC : NetworkManager
{
    public CustomizerData dataMessage = new CustomizerData();

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CustomizerData>(OnCreateCharacter);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

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

        NetworkServer.AddPlayerForConnection(conn, character);
    }
}
