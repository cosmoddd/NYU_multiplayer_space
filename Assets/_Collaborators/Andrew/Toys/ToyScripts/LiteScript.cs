using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LiteScript : NetworkBehaviour
{
    public Material[] materials;
    int whichMat;

    // Update is called once per frame

    void Interact()
    {
        CmdLightServer();
    }

    [Command(ignoreAuthority = true)]
    public void CmdLightServer()
    {
        RpcLightToggle();
    }

    [ClientRpc]
    public void RpcLightToggle()
    {

            whichMat++;
            if (whichMat>=materials.Length)
            {
                whichMat = 0;
            }

            GetComponent<Renderer>().material = materials[whichMat];
            GetComponent<ToyIdentityScript>().active = false;
    }
}
