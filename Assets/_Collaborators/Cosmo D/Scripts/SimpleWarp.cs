using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SimpleWarp : NetworkBehaviour
{

    public GameObject warpLocation;

    void OnTriggerEnter(Collider other)
    {
      if (isClient)
      {
        print(other.gameObject.name + "is trying to rise!");
        other.GetComponent<CharacterController>().enabled = false;
        other.transform.position = warpLocation.transform.position;
        other.GetComponent<CharacterController>().enabled = true;
      }
    }

}
