using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteScript : MonoBehaviour
{
    public Material[] materials;
    int whichMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ToyIdentityScript>().active)
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
}
