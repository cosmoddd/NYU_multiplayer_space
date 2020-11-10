using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_Die : NetworkBehaviour {
    public Rigidbody Rig;
    public Vector2 Force;
    public Vector2 RotationForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        CmdRoll(Z_Interactor.Main.transform.position);
    }

    [Command(ignoreAuthority = true)]
    public void CmdRoll(Vector3 Source)
    {
        Vector3 D = transform.position - Source;
        D.y = 0;
        D.Normalize();
        Rig.AddForce(D.x * Force.x, Force.y, D.z * Force.x);
        Rig.AddTorque(Random.Range(RotationForce.x, RotationForce.y), Random.Range(RotationForce.x, RotationForce.y), Random.Range(RotationForce.x, RotationForce.y));
    }
}
