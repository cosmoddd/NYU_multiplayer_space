using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotation : MonoBehaviour
{

    public float y_speed = 100.0f;  //makes public default speed for adjustments in editor

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, y_speed * Time.deltaTime * 2, 0);  //transform gameobject rotation of y axis

    }
}