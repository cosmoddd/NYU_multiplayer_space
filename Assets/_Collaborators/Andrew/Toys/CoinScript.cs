using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    bool flipping;
    public AnimationCurve airArc;

    Rigidbody RB;

    Vector3 groundedPos;

    void Start()
    {
        flipping = false;
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GetComponent<ToyIdentityScript>().active && !flipping)
        {
            Activate();
            GetComponent<ToyIdentityScript>().active = false;
        }

        if (flipping)
        {
            transform.position = Vector3.Lerp(transform.position,groundedPos+new Vector3(0,1,0),Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(90,0,0),Time.deltaTime * 10);
            if (transform.position.y > groundedPos.y+.9f)
            {
                RB.isKinematic = false;
                RB.AddForce(Vector3.up* 10, ForceMode.Impulse);
                RB.AddTorque(new Vector3(30,0,0),ForceMode.Impulse);
                flipping = false;
            }
        }

    }

    public void Activate()
    {
        flipping = true;
        RB.isKinematic = true;
        groundedPos = transform.position;
    }
}
