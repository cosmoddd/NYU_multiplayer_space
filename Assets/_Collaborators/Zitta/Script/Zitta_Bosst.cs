using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Local object
public class Zitta_Bosst : MonoBehaviour {
    public float Height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision C2D)
    {
        if (C2D.transform.GetComponent<MoveController>())
        {
            if (LocalCheck(C2D.transform.GetComponent<MoveController>()))
                Process(C2D.transform.GetComponent<MoveController>());
        }
    }

    public void Process(MoveController MC)
    {
        float jumpVelocity = Mathf.Sqrt(2 * MC.gravity * Height);
        //MC.velocityY = jumpVelocity;
    }

    public bool LocalCheck(MoveController MC)
    {
        if (MC)
            return true;
        return false;
    }
}
