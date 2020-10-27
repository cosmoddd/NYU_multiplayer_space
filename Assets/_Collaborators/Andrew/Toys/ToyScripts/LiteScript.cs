using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LiteScript : NetworkBehaviour
{
    public Material[] materials;

    [SyncVar]
    public int whichMat;

    public AudioSource lightSwitchAudio;

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

    public AudioClip[] lightSFXclips;

    [ClientRpc]
    void RpcLightToggle()
    {

            whichMat++;
            if (whichMat>=materials.Length)
            {
                whichMat = 0;
            }
            GetComponent<Renderer>().material = materials[whichMat];
            lightSwitchAudio.clip = lightSFXclips[UnityEngine.Random.Range(0,lightSFXclips.Length)];
            lightSwitchAudio.pitch = UnityEngine.Random.Range(.7f,1.3f);
            lightSwitchAudio.Play();
    }
}
