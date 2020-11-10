using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;

public class XylophoneMallet : NetworkBehaviour
{
    [FormerlySerializedAs("litMallet")]
    public Material litMalletMaterial;
    Material startingMaterial;
    AudioSource audioSource;
    MeshRenderer meshRenderer;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
        startingMaterial = meshRenderer.material;
    }

    [Command(ignoreAuthority = true)]
    public void CmdMalletHit() => RpcMalletHit();

    [ClientRpc]
    private void RpcMalletHit()
    {
        audioSource.Play();
        meshRenderer.material = litMalletMaterial;
        t = 8f;
    }

    float t;

    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
        }

        if(t <= 0 ||  !audioSource.isPlaying)
        {
            meshRenderer.material = startingMaterial;
        }
    }
}
