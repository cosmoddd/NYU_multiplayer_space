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
<<<<<<< HEAD
>>>>>>> 587cb20c4b8784dc89a87981a6a30c71958cdcc9:Assets/_Collaborators/Jay/BallBounce.cs
=======
>>>>>>> 8ee796a24760628b989409602c1129e27462ba8e:Assets/_Collaborators/Jay/BallBounce.cs
>>>>>>> 3868c6d4d9ab50cb41f631868775d082a8a86d06
    }


    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.CompareTag("killzone"))
        {
            Destroy(gameObject);
        }
    }



}
