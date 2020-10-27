using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AudioListenerController : NetworkBehaviour
{
    public GameObject objectListener;

    public override void OnStartAuthority()
    {
        objectListener.SetActive(true);
    }
}
