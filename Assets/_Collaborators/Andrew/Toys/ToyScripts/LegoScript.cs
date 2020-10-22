using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoScript : MonoBehaviour
{

    Vector3 potentialClick;

    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            if (hit.collider.tag == "Stud")
            {
                potentialClick = hit.collider.transform.parent.transform.position + new Vector3(0,1,0);
                if (GetComponent<ToyIdentityScript>().active)
                {
                    Debug.Log("LegoActivated");
                    transform.position = potentialClick;
                    transform.parent = hit.transform;
                    GetComponent<ToyIdentityScript>().active = false;
                }
            }
        }
    }
}
