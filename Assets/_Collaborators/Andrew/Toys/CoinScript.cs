using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CoinScript : NetworkBehaviour
{
    bool flipping;
    public AnimationCurve airArc;

    Rigidbody RB;

    Vector3 groundedPos;

    Vector3 startingPosition;

    void Start()
    {
        flipping = false;
        RB = GetComponent<Rigidbody>();
        startingPosition = this.transform.position;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Fall Zone"))
        {
            print("landed in the fall zone!");
            RB.velocity = Vector3.zero;
            transform.position = startingPosition;
        }
    }

    void Update()
    {
        if (flipping)
        {
            // transform.position = Vector3.Lerp(transform.position,groundedPos+new Vector3(0,1,0),Time.deltaTime * 5);
            // transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(90,0,0),Time.deltaTime * 10);
            // if (transform.position.y > groundedPos.y+.9f)
            {
                RB.isKinematic = false;
                RB.AddForce(Vector3.up* 10, ForceMode.Impulse);
                RB.AddTorque(new Vector3(60,0,0),ForceMode.Impulse);
                flipping = false;
            }
        }

    }

    [Command(ignoreAuthority = true)]
    public void CmdActivate() => RpcActivate();

    [ClientRpc]
    public void RpcActivate()
    {
        flipping = true;
        RB.isKinematic = true;
        groundedPos = transform.position;
    }
}
