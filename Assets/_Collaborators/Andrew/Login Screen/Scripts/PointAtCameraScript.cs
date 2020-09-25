using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtCameraScript : MonoBehaviour
{
    Vector3 lerpVector;
    public Transform lookPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lerpVector = Vector3.Lerp(lerpVector, lookPos.position + new Vector3(0, -10, 0),Time.deltaTime/.4f);

        transform.LookAt(lerpVector);
    }
}
