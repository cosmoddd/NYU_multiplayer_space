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
        if (other.gameObject.CompareTag("Player"))
        {
            //  print("KICK!!");
         Vector3 direction = (other.transform.position - transform.position).normalized;
         thisRigidbody.AddForce((-direction + new Vector3(0,.8f,0)) * kickForce, ForceMode.Impulse);
        }

        if(!contactSound.isPlaying)
        {
            contactSound.pitch = UnityEngine.Random.Range(.7f,1.3f);
            contactSound.Play();
        }

        if (other.gameObject.CompareTag("Fall Zone"))
        {
            print("landed in the fall zone!");
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
            if (Input.GetMouseButtonDown(0) && distanceToBall <15f)
            {
                print("CLICKING!!");
                Ray thisRay = localCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(thisRay.origin, thisRay.direction, out RaycastHit hitInfo, 100f))
                {
                    if (hitInfo.transform.gameObject == this.gameObject)
                    {
                        thisRigidbody.AddForce((thisRay.direction+new Vector3(0,upAngle,0)) * punchForce, ForceMode.Impulse);
                        if(!contactSound.isPlaying)
                        {
                            contactSound.pitch = UnityEngine.Random.Range(.7f,1.3f);
                            contactSound.Play();
                        }
                    }
                }
            }
         }
        // Debug.DrawRay(thisRay.origin, thisRay.direction *100, Color.red);
    }
}
