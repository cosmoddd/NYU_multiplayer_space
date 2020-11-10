using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TextLookAt : NetworkBehaviour
{
    public GameObject playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        print("Client connected");
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = playerCamera.transform.rotation;
    }
}
