using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerNYU : NetworkManager
{
    public SavedAvatarInfoScript avatarInfo;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // registers to be spawned 
        NetworkServer.RegisterHandler<SendPlayerMessage>(SpawnPlayerWithMessage);
    }

    // spawns this player w some data.  still not sure how this works but ok
    void SpawnPlayerWithMessage(NetworkConnection connection, SendPlayerMessage m)
    {
        GameObject spawn = Instantiate(playerPrefab, GetStartPosition().position, GetStartPosition().rotation);
        
        CharacterCustomizerScript customizer = spawn.GetComponent<CharacterCustomizerScript>();

        SavedAvatarInfoScript info = spawn.GetComponent<SavedAvatarInfoScript>();

        info.userName = m.userName;
        info.HeadMeshID = m.HeadMeshID;
        info.HatMeshID = m.HatMeshID;
        info.TorsoID = m.TorsoID;

        info.BodyColor = m.BodyColor;
        info.HatColor = m.HatColor;
        info.HeadColor = m.HeadColor;
        info.FootColor = m.FootColor;

        NetworkServer.AddPlayerForConnection(connection, spawn);
    }



    public override void OnClientConnect(NetworkConnection conn)
    {
        print("Connected!");
        base.OnClientConnect(conn);

        SendPlayerMessage thisMessage = new SendPlayerMessage();

        thisMessage.userName = avatarInfo.userName;
        thisMessage.HeadMeshID = avatarInfo.HeadMeshID;
        thisMessage.HatMeshID = avatarInfo.HatMeshID;
        thisMessage.TorsoID = avatarInfo.TorsoID;

        thisMessage.BodyColor = avatarInfo.BodyColor;
        thisMessage.HatColor = avatarInfo.HatColor;
        thisMessage.HeadColor = avatarInfo.HeadColor;
        thisMessage.FootColor = avatarInfo.FootColor;

        conn.Send(thisMessage);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        print("disconnecting");
        base.OnClientDisconnect(conn);
    }

    public void SetServerAddress(string s)
    {
        networkAddress = s;
    }

}
