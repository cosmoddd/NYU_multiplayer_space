using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LiteScript : MonoBehaviour
{
    public Material[] materials;
    int whichMat;

    // Update is called once per frame

    void Interact()
    {
        LightToggle();
    }

    public void LightToggle()
    {
        // if (GetComponent<ToyIdentityScript>().active)
        // {
            whichMat++;
            if (whichMat>=materials.Length)
            {
                whichMat = 0;
            }

            GetComponent<Renderer>().material = materials[whichMat];
            GetComponent<ToyIdentityScript>().active = false;
        // }
    }
}
