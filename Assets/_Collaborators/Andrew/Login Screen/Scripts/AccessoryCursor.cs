using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccessoryCursor : MonoBehaviour
{
    Vector3 lerpNormal;

    public Transform player;

    float rot;
    float rotLerp;

    public GameObject cylinder;
    public Transform parenter;
    // Start is called before the first frame update
    void Start()
    {
        rot = 180;
        rotLerp = 180;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.collider != null && !Input.GetKey(KeyCode.Mouse1))
        {
            transform.position = hit.point;

            lerpNormal = Vector3.Lerp(lerpNormal,hit.normal,Time.deltaTime * 15);
            transform.up = lerpNormal;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Instantiate(cylinder, transform.position, transform.rotation,parenter);
            }

        }
        else
        {
            transform.position = Vector3.one * 999;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            rot -= Input.GetAxisRaw("Mouse X") * Time.deltaTime * 3000;
            rotLerp = Mathf.Lerp(rotLerp,rot,Time.deltaTime * 7);
        }
        player.rotation = Quaternion.Euler(new Vector3(0, rotLerp, 0));
    }
}
