using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {

            if (hit.distance < 1 && !hit.collider.isTrigger)
            {
                Vector3 fDist = -(hit.normal) - transform.forward;
                Vector3 nDist = hit.normal - fDist;
                transform.forward = nDist;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            }
        }

        transform.position += transform.forward * Time.deltaTime * 4.5f;
    }
}
