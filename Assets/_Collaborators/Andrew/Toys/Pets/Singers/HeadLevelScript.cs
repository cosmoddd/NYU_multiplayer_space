using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLevelScript : MonoBehaviour
{
    public Transform neck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = neck.position;
        transform.rotation = Quaternion.Euler(new Vector3(neck.rotation.eulerAngles.x, neck.rotation.eulerAngles.y+180, neck.rotation.eulerAngles.z));
    }
}
