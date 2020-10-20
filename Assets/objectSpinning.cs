using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class objectSpinning : NetworkBehaviour
{

    public float multiplier;
    public float speed;
    public GameObject obstacleSpin;
    
    public float RZ;
    public float z;
    // Start is called before the first frame update
    void Start()
    {
       
        obstacleSpin.transform.Rotate(0, 0f, RZ,Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        obstacleSpin.transform.Rotate(0f, 0f, RZ + speed * Time.deltaTime * multiplier, Space.Self);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag ("Player"))
        {
            
        }
    }
}
