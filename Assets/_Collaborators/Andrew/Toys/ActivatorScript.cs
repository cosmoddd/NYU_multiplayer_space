using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorScript : MonoBehaviour
{
    public ToyIdentityScript toy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (hit.collider.GetComponent<ToyIdentityScript>()!=null)
                {
                    if (!hit.collider.GetComponent<ToyIdentityScript>().active)
                    {
                        hit.collider.GetComponent<ToyIdentityScript>().active = true;
                    }
                }
            }
        }


    }
}
