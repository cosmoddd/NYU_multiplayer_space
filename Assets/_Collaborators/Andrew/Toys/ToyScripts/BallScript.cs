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
    public float distanceToBall;
    public float upAngle = 1.5f;

    public Collider[] overlappingItems;
    public LayerMask overlappingTargetLayer;

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
        print(netIdentity.connectionToClient.identity.transform.name);
        
        if (other.gameObject.transform.parent && other.gameObject.transform.parent.CompareTag("Player"))
        {   

        
            // change authority if it's you
            if (other.gameObject.transform.parent.GetComponent<NetworkIdentity>() 
                && other.gameObject.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
              if (!netIdentity.hasAuthority)
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

    private void OnTriggerEnter(Collider other)
    {
        print($"{other.name} hit it!");

        if (other.tag == "Goal")
        {
            other.GetComponent<GoalpostScript>().CmdScoredGoal();
            thisRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
        }
    }

    public float drawRadius = 10f;
    
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
  
    [Command(ignoreAuthority=true)]
    void CmdMoveIntoSphere(NetworkIdentity playerConn)
    {
          
      netIdentity.RemoveClientAuthority();
      netIdentity.AssignClientAuthority(playerConn.connectionToClient);
    }  


    public void Update()
    {
      overlappingItems = Physics.OverlapSphere(transform.position, drawRadius, overlappingTargetLayer);

      if (!localCamera) return;

      if (localPlayer)
      {
          distanceToBall = (this.transform.position - localPlayer.transform.position).magnitude;
      }

      // DetermineShortestDistance();

      /*         
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
      */
    }

    public List <float> distancesToBall = new List<float>();
    public float minFloat;
    public GameObject closestCharacter;
    public GameObject closestCharacterSaved;



    void DetermineShortestDistance()
    {
      if (overlappingItems.Length>0)
      {
        // distancesToBallArray = new List<float>();
        // distancesToBall.Add()
        distancesToBall = new List<float>();

        for (int i = 0; i < overlappingItems.Length; i++)
        {
          distancesToBall.Add((this.transform.position - overlappingItems[i].transform.position).magnitude);
        }

        minFloat = distancesToBall.Min();
        closestCharacter = overlappingItems[distancesToBall.IndexOf(minFloat)].gameObject;  // waaaaa!?!?!?!?


          if((closestCharacter != closestCharacterSaved))
          {
            closestCharacterSaved = closestCharacter;
            CmdMoveIntoSphere(closestCharacter.GetComponent<NetworkIdentity>());
            print("Setting new network id for..." + closestCharacter.name);
          }
        }
      
      
      if (overlappingItems.Length == 0)
      {
        distancesToBall.Clear();
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

    void BasicKick(Vector3 contactDirection)
    {
        print("kicking in a direction!!");
        thisRigidbody.AddForce((contactDirection + new Vector3(0, upAngle, 0)) * punchForce, ForceMode.Impulse);

    }
    
}
