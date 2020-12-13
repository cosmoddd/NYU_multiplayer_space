using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        // Slowly rotate the object arond its X axis at 1 degree/second.
        transform.Rotate(0, (Time.deltaTime * 10), 0);

        // ... at the same time as spinning it relative to the global
        // Y axis at the same speed.
        //transform.Rotate(0, Time.deltaTime, 0, Space.World);
    }
}
