using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    Vector3 startPosition;
    public Light doorLight;
    float lightIntesne;
    // Start is called before the first frame update
    void Start()
    {
        lightIntesne = doorLight.intensity;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + new Vector3(0,Mathf.Sin(Time.time),0);
        doorLight.intensity = lightIntesne + Mathf.Sin(Time.time*2)*1000;
    }
}
