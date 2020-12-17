using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Linq;

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
    float distanceToBall;
    public float upAngle = 1.5f;

    public LayerMask clickMask;

    public GameObject particlePrefab;

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

        if (other.gameObject.transform.parent && other.gameObject.transform.parent.CompareTag("Player"))
        {   

        
            // change authority if it's you
            if (other.gameObject.transform.parent.GetComponent<NetworkIdentity>() 
                && other.gameObject.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                  if (!netIdentity.hasAuthority && isClientOnly)
                  {
                    print("KICK WITHOUT AUTHORITY AND ADD IT!!");
                    CmdMoveIntoSphere(other.gameObject.transform.parent.GetComponent<NetworkIdentity>());
                    Vector3 kickDirection = (other.transform.position - transform.position).normalized;
                    BasicKick(kickDirection);
                  }
                  
                  
                  if (netIdentity.hasAuthority)
                  {
                    print("KICK WITH AUTHORITY!!");
                    Vector3 kickDirection = (other.transform.position - transform.position).normalized;
                    BasicKick(kickDirection);
                  }

                  if (isServer)
                  {
                    print("KICK SERVER POWER!!");
                    netIdentity.RemoveClientAuthority();
                    Vector3 kickDirection = (other.transform.position - transform.position).normalized;
                    BasicKick(kickDirection);
                  }
                }
            
          // play sound in any case
          if (!contactSound.isPlaying || contactSound.time > .4f)
          {
              contactSound.pitch = UnityEngine.Random.Range(.8f, 1.3f);
              contactSound.Play();
          }

          foreach (ContactPoint c in other.contacts)
          {
              Instantiate(particlePrefab, c.point, Quaternion.identity);
          }
        }


        if (other.gameObject.CompareTag("Fall Zone"))
        {
            // print("landed in the fall zone!");
            thisRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

    [Command(ignoreAuthority=true)]
    void CmdMoveIntoSphere(NetworkIdentity playerConn)
    {
          
      netIdentity.RemoveClientAuthority();
      netIdentity.AssignClientAuthority(playerConn.connectionToClient);
    }  


    
    private void OnTriggerEnter(Collider other)
    {
        // print($"{other.name} hit it!");

        if (other.tag == "Goal")
        {
            other.GetComponent<GoalpostScript>().CmdScoredGoal();
            thisRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

    float drawRadius = 10f;
    
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, drawRadius);
    }

    public override void OnStartAuthority()
    {
      print("You have authority over the "+ this.gameObject.name);
    }

  public override void OnStopAuthority()
  {
    base.OnStopAuthority();
    print("You NO LONGER have authority over the "+ this.gameObject.name);

  }
  
    // if you don't feel like kicking the ball you can also just click on it.
    public void Update()
    {
      if (!localCamera) return;

      if (localPlayer)
      {
          distanceToBall = (this.transform.position - localPlayer.transform.position).magnitude;

          if (isClickable)
          {
              if (Input.GetMouseButtonDown(0) && distanceToBall < 15f)
              {
                  Ray thisRay = localCamera.ScreenPointToRay(Input.mousePosition);

                  if (Physics.Raycast(thisRay.origin, thisRay.direction, out RaycastHit hitInfo, 100f, clickMask) 
                      && hitInfo.transform.CompareTag("Ball"))
                  { 
                        Instantiate(particlePrefab, hitInfo.point, Quaternion.identity);
                        AudioClick();
                        CmdClickBall(thisRay.direction);
                  }
              }
          } 
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

   void AudioClick()
    {
        if (!contactSound.isPlaying || contactSound.time > .4f)
        {
            contactSound.pitch = UnityEngine.Random.Range(.8f, 1.3f);
            contactSound.Play();
        }
    }


    [Command(ignoreAuthority = true)]
    void CmdClickBall(Vector3 _contactDirection)
    {
      
        RpcClickKick(_contactDirection);
        // RpcAudioHit();
    }

    [ClientRpc]
    void RpcClickKick(Vector3 _contactDirection)
    {
      thisRigidbody.AddForce((_contactDirection + new Vector3(0, upAngle, 0)) * (punchForce*.66f), ForceMode.Impulse);
    }

    void BasicKick(Vector3 contactDirection)
    {
        print("kicking in a direction!!");
        thisRigidbody.AddForce((contactDirection + new Vector3(0, upAngle, 0)) * punchForce, ForceMode.Impulse);
    }
    
}
