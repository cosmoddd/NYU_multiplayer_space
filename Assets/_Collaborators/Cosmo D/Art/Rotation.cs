using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float thisRotation = 5f;

    void Update()
    {
       transform.Rotate(new Vector3(transform.rotation.eulerAngles.x, thisRotation, transform.rotation.eulerAngles.z) * Time.deltaTime);
    }
}
