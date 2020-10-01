using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Transform target;

    [Header("Player Settings")]
    public float mouseSensitivity = 10.0f;
    public float changeCamDistSense = 0.3f;

    [Header("Cursor Settings")]
    public bool cursorVisible;
    public bool clickToMove = true;

    [Header("Camera settings")]
    public Vector2 pitchMinMax = new Vector2(-45, 90);

    [Header("Distance from target settings")]
    [FormerlySerializedAs("distFromTarget")]
    public float initialCamDist = 5.0f;
    public float minCamDist;
    public float maxCamDist;
    public float distChangeRate;

    private float pitch, yaw;

    private float currentCamDist;
    private float targetCamDist;

    public Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (!cursorVisible)
        {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cursorVisible;
        }  

        currentCamDist = initialCamDist;
        targetCamDist = initialCamDist;
    }

    private void Update()
    {
        // toggle clickToMove option
        if(Input.GetKeyDown(KeyCode.Z))
        {
            clickToMove = clickToMove ? false : true;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!clickToMove || Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // yaw for looking side to side, pitch for looking up and down
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            transform.eulerAngles = new Vector3(pitch, yaw);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = cursorVisible;
        }

        // scroll to change camera distance
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            targetCamDist -= changeCamDistSense * Input.mouseScrollDelta.y;
            targetCamDist = Mathf.Clamp(targetCamDist, minCamDist, maxCamDist);
            currentCamDist = Mathf.Lerp(currentCamDist, targetCamDist, distChangeRate);
        }

        transform.position = target.position + cameraOffset - transform.forward * currentCamDist;
    }
}
