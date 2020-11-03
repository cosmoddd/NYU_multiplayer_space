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
<<<<<<< HEAD:Assets/BallBounce.cs
        bouncy.bounciness = 1 * Random.Range(minmulti, maxmulti);
=======
        bouncy.bounciness = Random.Range(3f, 7f)/7f;
>>>>>>> 8ee796a24760628b989409602c1129e27462ba8e:Assets/_Collaborators/Jay/BallBounce.cs
    }


    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.CompareTag("killzone"))
        {
            Destroy(gameObject);
        }
    }



}
