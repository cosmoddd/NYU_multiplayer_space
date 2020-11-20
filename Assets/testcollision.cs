using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class testcollision : NetworkBehaviour


{
   
    public float eject;
    public Transform playerPrefab;
    public Transform pTran;

    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = this.transform.parent.GetComponent<Transform>();
        pTran = playerPrefab.transform;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("obstacles1"))
        {
            Debug.Log("player detected");

            float tranx = pTran.position.x;
            float trany = pTran.position.y;
            float tranz = pTran.position.z;

            pTran.transform.position = new Vector3(tranx-- /eject * Time.deltaTime, trany, tranz) ;
        }
    }
}
