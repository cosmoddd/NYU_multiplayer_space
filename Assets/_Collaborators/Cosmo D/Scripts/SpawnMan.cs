using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnMan : NetworkBehaviour
{

    public GameObject spawnedThing;

    public override void OnStartServer()
    {
        if (isServer)
        {
            GameObject spawned = Instantiate(spawnedThing, this.transform.position, Quaternion.identity);
            NetworkServer.Spawn(spawned);
        }

    }


}
