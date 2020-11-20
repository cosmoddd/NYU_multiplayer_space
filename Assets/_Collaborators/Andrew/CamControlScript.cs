using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControlScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    float xrot = 88.86f;
    float yrot = 18.22f;

    // Update is called once per frame
    void Update()
    {
        yrot -= Input.GetAxisRaw("Mouse Y") * 300 * Time.deltaTime;
        xrot += Input.GetAxisRaw("Mouse X") * 300 * Time.deltaTime;

        yrot = Mathf.Clamp(yrot,-90,90);

        transform.rotation = Quaternion.Euler(yrot,xrot,0);

        transform.position += ((transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical")))*Time.deltaTime * 10;
    }
}
