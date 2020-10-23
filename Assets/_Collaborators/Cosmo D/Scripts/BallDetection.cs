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
            // print(charController.velocity.magnitude);
            Vector3 kickDirection = (hit.transform.position - transform.position).normalized;
            CmdKickBall(hit.gameObject,charController.velocity.magnitude, kickDirection);
            // hit.gameObject.GetComponent<Rigidbody>().velocity = charController.velocity.magnitude * kickForce;
            // hit.gameObject.GetComponent<Rigidbody>().AddForce((-kickDirection + new Vector3(0,kickHeight,0)) * kickForce * charController.velocity.magnitude, ForceMode.Impulse);
        }
    
    
    }

    [Command(ignoreAuthority = true)]
    public void CmdKickBall(GameObject ball, float magnitude, Vector3 kickDirection)
    {
        // ball.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        RpcKickBall(ball, magnitude, kickDirection);
    }


    [ClientRpc]
    void RpcKickBall(GameObject ball, float magnitude, Vector3 _kickDirection)
    {
        // print("KICK THAT BALL, Balls!");
        ball.GetComponent<BallScript>().CmdAudioHit();
        ball.GetComponent<Rigidbody>().AddForce((-_kickDirection + new Vector3(0,kickHeight,0)) * kickForce * magnitude, ForceMode.Impulse);
    }
}
