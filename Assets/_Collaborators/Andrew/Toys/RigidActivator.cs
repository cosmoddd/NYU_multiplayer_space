using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RigidActivator : NetworkBehaviour
{
    [SyncVar]
    public GameObject Toy;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (hit.collider.GetComponent<ToyIdentityScript>() != null)
                {
                    Toy = hit.collider.gameObject;
                    CmdSendMessage(Toy);
                }
            }
        }
    }

    [Command]
    void CmdSendMessage(GameObject toy)
    {
        RpcPrintMessage(toy);
    }

    [ClientRpc]
    void RpcPrintMessage(GameObject toy)
    {
        if (!toy.GetComponent<ToyIdentityScript>().active)
        {           
            toy.GetComponent<ToyIdentityScript>().active = true;
        }
    }
}
