using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhysicsKick : NetworkBehaviour
{
    public float kickForce = .33f;
    public float kickHeight = .5f;
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
