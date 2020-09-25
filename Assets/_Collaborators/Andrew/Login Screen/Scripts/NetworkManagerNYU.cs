using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerNYU : NetworkManager
{

    public SavedAvatarInfoScript avatarInfo;
    //public CharacterCustomizerScript customizer;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<SendPlayerMessage>(SpawnPlayerWithMessage);
    }

    void SpawnPlayerWithMessage(NetworkConnection connection, SendPlayerMessage m)
    {
        GameObject spawn = Instantiate(playerPrefab);
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

        customizer.assignFromSavedInfo();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
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

}
