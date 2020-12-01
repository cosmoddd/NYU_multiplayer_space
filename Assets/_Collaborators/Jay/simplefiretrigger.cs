using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class simplefiretrigger : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.gameObject.SetActive(true);

        } else if (Input.GetKeyDown(KeyCode.P))

        {
            this.gameObject.SetActive(false);
        }
    }
}
