using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SavedAvatarInfoScript : NetworkBehaviour
{
    [SyncVar]
    public string userName;
    [SyncVar]    
    public int HeadMeshID;
    [SyncVar]
    public int FeetMeshID;
    [SyncVar]
    public int HatMeshID;
    [SyncVar]
    public int TorsoID;

    //public float[] TorsoPresets;

    [SyncVar]
    public Vector3 BodyColor;
    [SyncVar]
    public Vector3 HeadColor;
    [SyncVar]
    public Vector3 HatColor;
    [SyncVar]
    public Vector3 FootColor;
}
