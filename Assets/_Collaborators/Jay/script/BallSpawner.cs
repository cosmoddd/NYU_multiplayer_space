using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallSpawner : NetworkBehaviour
{



    public GameObject theBall;
    public float rate = 3.0f;

    

    float progress = 0.0f;

    void Spawn()
    {
        GameObject next = Instantiate(theBall, transform.position, transform.rotation);
        next.transform.parent = transform;
        next.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        progress += rate * Time.deltaTime;
        if (progress >= 3.0f)
        {
            Spawn();
            progress -= 3.0f;
        }
    }
}
