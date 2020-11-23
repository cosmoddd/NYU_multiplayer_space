using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    Vector3 startPos;
    public Rigidbody thisRigidbody;
    public float kickForce = 10f;
    public float punchForce = 5f;
    public AudioSource contactSound;
    public bool isClickable;

    public GameObject localPlayer;
    public Camera localCamera;
    public float distanceToBall;
    public float upAngle = 1.5f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        startPos = transform.position;
        MoveController.controllerAndCameraInit += GetControllerAndCamera;

    }

    public override void OnStopClient()
    {
        MoveController.controllerAndCameraInit -= GetControllerAndCamera;
    }

    void GetControllerAndCamera(GameObject _localPlayer, GameObject _localCamera)
    {
        localPlayer = _localPlayer;
        localCamera = _localCamera.GetComponentInChildren<Camera>();
    }

    void OnCollisionEnter(Collision other)
    {

        //  print("KICK?");
        /* if (other.gameObject.CompareTag("Player"))
        {
            print("KICK!!");
            Vector3 kickDirection = (other.transform.position - transform.position).normalized;

            CmdContactBall(kickDirection);
        } */


        if (other.gameObject.CompareTag("Fall Zone"))
        {
            // print("landed in the fall zone!");
            thisRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goal")
        {
            other.GetComponent<GoalpostScript>().CmdScoredGoal();
            thisRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

    public void Update()
    {
        if (!localCamera) return;

        if (localPlayer)
        {
            distanceToBall = (this.transform.position - localPlayer.transform.position).magnitude;
        }

        if (isClickable)
        {
            if (Input.GetMouseButtonDown(0) && distanceToBall < 15f)
            {
                Ray thisRay = localCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(thisRay.origin, thisRay.direction, out RaycastHit hitInfo, 100f))
                {
                    if (hitInfo.transform.gameObject == this.gameObject)
                    {
                        CmdContactBall(thisRay.direction);
                    }
                }
            }
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdKickBall(Vector3 kickDirection)
    {
        thisRigidbody.AddForce((-kickDirection + new Vector3(0, .8f, 0)) * kickForce, ForceMode.Impulse);
        RpcKickBall(kickDirection);
    }


    [ClientRpc]
    void RpcKickBall(Vector3 _kickDirection)
    {
        if (!contactSound.isPlaying || contactSound.time > .4f)
        {
            contactSound.pitch = UnityEngine.Random.Range(.8f, 1.3f);
            contactSound.Play();
        }

    }

    [Command(ignoreAuthority = true)]
    public void CmdAudioHit() => RpcAudioHit();


    [ClientRpc]
    void RpcAudioHit()
    {
        if (!contactSound.isPlaying || contactSound.time > .4f)
        {
            contactSound.pitch = UnityEngine.Random.Range(.8f, 1.3f);
            contactSound.Play();
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdContactBall(Vector3 contactDirection) => RpcContactBall(contactDirection);

    [ClientRpc]
    void RpcContactBall(Vector3 contactDirection)
    {
        print("CLICKING!!");
        thisRigidbody.AddForce((contactDirection + new Vector3(0, upAngle, 0)) * punchForce, ForceMode.Impulse);
        if (!contactSound.isPlaying || contactSound.time > .4f)
        {
            contactSound.pitch = UnityEngine.Random.Range(.8f, 1.3f);
            contactSound.Play();
        }

    }
}
