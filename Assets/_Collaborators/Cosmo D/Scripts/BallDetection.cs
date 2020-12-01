using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BallDetection : NetworkBehaviour
{
    // public PhysicsKick physicsKickScript;
    public CharacterController charController;

    public float kickForce = .33f;
    public float kickHeight = .5f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ball"))
        {
            Vector3 kickDirection = (hit.transform.position - transform.position).normalized;
            CmdKickBall(hit.gameObject,charController.velocity.magnitude, kickDirection);
        }
    
    
    }

    [Command(ignoreAuthority = true)]
    public void CmdKickBall(GameObject ball, float magnitude, Vector3 kickDirection)
    {
        RpcKickBall(ball, magnitude, kickDirection);
    }


    [ClientRpc]
    void RpcKickBall(GameObject ball, float magnitude, Vector3 _kickDirection)
    {
        ball.GetComponent<BallScript>().CmdAudioHit();
        ball.GetComponent<Rigidbody>().AddForce((-_kickDirection + new Vector3(0,kickHeight,0)) * kickForce * magnitude, ForceMode.Impulse);
    }
}
