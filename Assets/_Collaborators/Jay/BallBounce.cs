using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BallBounce : NetworkBehaviour
{

    public SphereCollider sc;
    public PhysicMaterial bouncy;
    


    private void Start()
    {
        sc = GetComponent<SphereCollider>();
        bouncy = new PhysicMaterial();
        bouncy = sc.material;
        bouncy.bounciness = Random.Range(3, 7);
    }


    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.CompareTag("killzone"))
        {
            Destroy(gameObject);
        }
    }



}
