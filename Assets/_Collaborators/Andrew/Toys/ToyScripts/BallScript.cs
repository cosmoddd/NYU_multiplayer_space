using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    Vector3 startPos;
    public Rigidbody thisRigidbody;
    public float kickForce = 10f;

  public override void OnStartClient()
   {
        base.OnStartClient();

        startPos = transform.position;

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
}
