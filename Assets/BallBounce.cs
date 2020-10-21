using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BallBounce : NetworkBehaviour
{

    public SphereCollider sc;
    public PhysicMaterial bouncy;
    public float minmulti;
    public float maxmulti;


    private void Start()
    {
        sc = GetComponent<SphereCollider>();
        bouncy = new PhysicMaterial();
        bouncy = sc.material;
        bouncy.bounciness = 1 * Random.Range(minmulti, maxmulti);
    }


    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.CompareTag("killzone"))
        {
            Destroy(gameObject);
        }
    }



}
