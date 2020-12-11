using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoomScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<MoveController>().bInChangeRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<MoveController>().bInChangeRoom = false;
        }
    }
}
