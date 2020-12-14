using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SendPlayerMessage : NetworkMessage
{
    public string userName;

    public int HeadMeshID;
    public int FeetMeshID;
    public int HatMeshID;
    public int TorsoID;

    public Vector3 BodyColor;
    public Vector3 HeadColor;
    public Vector3 HatColor;
    public Vector3 FootColor;
}
