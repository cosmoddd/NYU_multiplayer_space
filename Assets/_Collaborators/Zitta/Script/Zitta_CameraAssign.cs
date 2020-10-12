using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_CameraAssign : MonoBehaviour {
    public bool Assigned;

    public void Awake()
    {
        if (Camera.main && !Assigned)
        {
            Assign();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main && !Assigned)
        {
            Assign();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main && !Assigned)
        {
            Assign();
        }
    }

    public void Assign()
    {
        gameObject.transform.parent = Camera.main.transform;
        gameObject.transform.localPosition = new Vector3();
        Assigned = true;
        Destroy(this);
    }
}
