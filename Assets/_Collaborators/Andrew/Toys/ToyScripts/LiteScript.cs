using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LiteScript : NetworkBehaviour
{
    public Material[] materials;

    [SyncVar]
    public int whichMat;

  public override void OnStartClient()
  {
    base.OnStartClient();

    GetComponent<Renderer>().material = materials[whichMat];  
  }

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
    void RpcLightToggle()
    {

            whichMat++;
            if (whichMat>=materials.Length)
            {
                whichMat = 0;
            }

            GetComponent<Renderer>().material = materials[whichMat];
            // GetComponent<ToyIdentityScript>().active = false;
    }
}
