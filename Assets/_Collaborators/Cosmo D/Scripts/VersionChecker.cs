using System;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Mirror;

public class VersionChecker : NetworkBehaviour
{
    public static Func <int> GetVersionFunc;
    public IntVariable gameVersion;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        int serverVersion = (int)GetVersionFunc?.Invoke();
        print($"Server version is: {serverVersion}.  Client version is {gameVersion.Value}.");

        if(serverVersion != gameVersion.Value)
        {
            // disconnect!
            // load scene with "YOU NEED NEW VERSION TEXT"
        }

        // if version matches, keep on. 


    }

}
