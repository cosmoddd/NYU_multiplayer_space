using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class simplefiretrigger : MonoBehaviour
{

    public GameObject Fire;
    // Start is called before the first frame update
    void Start()
    {
        Fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Fire.SetActive(true);

        } else if (Input.GetKeyDown(KeyCode.O))
        {
            Fire.SetActive(false);
        }
    }
}
