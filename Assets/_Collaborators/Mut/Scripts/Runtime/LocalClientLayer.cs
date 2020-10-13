using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityAtoms.BaseAtoms;

public class LocalClientLayer : NetworkBehaviour
{
    [SerializeField] private IntConstant layer;
    public override void OnStartClient()
    {
        gameObject.layer = layer.Value;
    }
}
