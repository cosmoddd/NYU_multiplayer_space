using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_CameraAssign : MonoBehaviour {
    public bool Assigned;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Zitta_CameraDetection.Main && !Assigned)
        {
            Assign();
        }
    }

    public void Assign()
    {
        gameObject.transform.parent = Zitta_CameraDetection.Main.transform;
        gameObject.transform.localPosition = new Vector3();
        Assigned = true;
        Destroy(this);
    }
}
