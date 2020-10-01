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
  public float distFromTargetChangeSensitivity;

  [Header("Cursor Settings")]
  public bool cursorVisible;
  public bool clickToMove = true;

  [Header("Camera settings")]
  public Vector2 pitchMinMax = new Vector2(-45, 90);

  [Header("Distance from target settings")]
  [FormerlySerializedAs("distFromTarget")]
  public float initialDistFromTarget = 5.0f;
  public float minDistFromTarget;
  public float maxDistFromTarget;
  public float distFromTargetChangeSmooth;

  private float pitch, yaw;

  private float currentDistFromTarget;
  // The distance from the target we are currently lerping towards
  private float expectedDistFromTarget;

  public Vector3 cameraOffset;
  Vector3 startingEuler;

  // Start is called before the first frame update
  void Start()
  {
    if (!cursorVisible)
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = cursorVisible;
    }

    currentDistFromTarget = initialDistFromTarget;
    expectedDistFromTarget = initialDistFromTarget;

    startingEuler = transform.eulerAngles;
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

      
      transform.eulerAngles = new Vector3(pitch, yaw) + startingEuler; // starting offset
    }
    else if (Input.GetKeyUp(KeyCode.Mouse1))
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = cursorVisible;
    }

    if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
    {
      expectedDistFromTarget -= distFromTargetChangeSensitivity * Input.mouseScrollDelta.y;
      expectedDistFromTarget = Mathf.Clamp(expectedDistFromTarget, minDistFromTarget, maxDistFromTarget);
      currentDistFromTarget = Mathf.Lerp(currentDistFromTarget, expectedDistFromTarget, distFromTargetChangeSmooth);
    }

    transform.position = target.position + cameraOffset - transform.forward * currentDistFromTarget;
  }
}
