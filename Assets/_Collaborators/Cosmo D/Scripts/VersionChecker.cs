using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Mirror;

public class VersionChecker : NetworkBehaviour
{
    public static Func <int> GetVersionFunc;
    public IntVariable gameVersion;
    public BoolVariable clientUpdateAvailable;
    NetworkManagerGC managerGC;
    int serverVersion;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        while (managerGC==null)
        {
            managerGC = FindObjectOfType<NetworkManagerGC>();
        }

        print("GET SERVER VERSION");

        StartCoroutine(VersionCheck());

    }

    IEnumerator VersionCheck()
    {
        yield return new WaitForSeconds(1f);
        print($"Server version is... {gameVersion.Value}.");
        
            serverVersion = GetVersionFunc.Invoke();
            print($"Server version is: {serverVersion}.  Client version is {gameVersion.Value}.");

            if(serverVersion != gameVersion.Value)
            {
                print("WRONG VERSION NUMBER!");
                clientUpdateAvailable.Value = true;
                managerGC.StopClient();
            }
    }
}
