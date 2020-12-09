using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityAtoms.BaseAtoms;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Transform target;

    public Transform viewCamera;

    [Header("Player Settings")]
    public float mouseSensitivity = 1.0f; // scales our sensitivity
    [SerializeField] float maxSensitivity = 5.0f; // actual max sensitivity value
    public float distFromTargetChangeSensitivity;

    [Header("Cursor Settings")]
    public bool cursorVisible;
    public bool clickToMove = true;

    [Header("Camera settings")]
    public Vector2 pitchMinMax = new Vector2(-45, 90);
    public float scrollSensitivity = 1f;

    [Header("Distance from target settings")]
    [FormerlySerializedAs("distFromTarget")]
    public float initialCamDist = 5.0f;
    public float minCamDist;
    public float maxCamDist;
    public float camDistSmoothRate;

    public float targetSmoothTime;
    Vector3 targetSmoothVelocity;

    private float pitch, yaw;

    public float currentCamDist;
    // The distance from the target we are currently lerping towards
    private float targetCamDist;

    bool bFreeCam = true;

    public Vector3 cameraOffset;
    Vector3 startingEuler;

    [Header("Atoms")]
    public BoolVariable inChatMode;

    public BoolVariable bInvertY;

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

        startingEuler = transform.eulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            clickToMove = !clickToMove;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            bFreeCam = !bFreeCam;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if in chat mode (or menu) return
        if (inChatMode.Value)
        {
            return;
        }         

        if (!clickToMove || Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // yaw for looking side to side, pitch for looking up and down
            yaw += Input.GetAxis("Mouse X") * (maxSensitivity * mouseSensitivity);

            if(bInvertY.Value)
            {
                pitch += Input.GetAxis("Mouse Y") * (maxSensitivity * mouseSensitivity);
            }
            else
            {
                pitch -= Input.GetAxis("Mouse Y") * (maxSensitivity * mouseSensitivity);
            }
            
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            transform.eulerAngles = new Vector3(pitch, yaw) + startingEuler; // starting offset

            if(!bFreeCam)
            {
                target.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
            }
        }

        // added immediate feedback for pressing Y to re-enable mouse  - Cosmo D
        if (clickToMove && !Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = cursorVisible;            
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = cursorVisible;
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            targetCamDist -= distFromTargetChangeSensitivity * Input.mouseScrollDelta.y * scrollSensitivity;
            targetCamDist = Mathf.Clamp(targetCamDist, minCamDist, maxCamDist);
            currentCamDist = Mathf.Lerp(currentCamDist, targetCamDist, camDistSmoothRate);
        }

        // set the local forward distance of childed camera
        Vector3 cameraBoomOffset = new Vector3(0.0f, 0.0f, -currentCamDist);
        viewCamera.localPosition = cameraBoomOffset;

        // SmoothDamp for camera lag
        if (target)
        {
            Vector3 newPosition = target.position + cameraOffset;
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref targetSmoothVelocity, targetSmoothTime);
        }
    }
}
