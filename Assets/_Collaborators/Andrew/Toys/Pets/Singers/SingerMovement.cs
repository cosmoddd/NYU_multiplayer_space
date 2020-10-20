using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingerMovement : MonoBehaviour
{
    public LayerMask mask;
    
    public float movementSpeed = 4.5f;
    public Vector3 rayOriginOffset;
    public float rayInteractionDistance =1;

    public bool movementPaused = false;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + rayOriginOffset, transform.forward*10f, Color.red);

        if (movementPaused) return;

        if (Physics.Raycast(transform.position + rayOriginOffset, transform.forward, out RaycastHit hit, 10f, mask))
        {

            if (hit.distance < rayInteractionDistance && !hit.collider.isTrigger)
            {
                // print($"{this.gameObject.name} hit a {hit.transform.name}");
                Vector3 fDist = -(hit.normal) - transform.forward;
                Vector3 nDist = hit.normal - fDist;
                transform.forward = nDist;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            }
        }

        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }

    public void StopMoving(float s)
    {
        StartCoroutine(FreezeMovement(s));
    }

    IEnumerator FreezeMovement(float _seconds)
    {
        movementPaused = true;
        yield return new WaitForSeconds(_seconds);
        movementPaused = false;
    }
}
