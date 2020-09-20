using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Transform target;
  public float distFromTarget = 5.0f;
  public float mouseSensitivity = 10.0f;
  public Vector2 pitchMinMax = new Vector2(-45, 90);

  float pitch, yaw;

  public bool cursorVisible;
  public bool clickToMove = true;

  // Start is called before the first frame update
  void Start()
  {
    if (!cursorVisible)
    {

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = cursorVisible;
    }
  }

  // Update is called once per frame
  void LateUpdate()
  {
    if (!clickToMove || Input.GetKey(KeyCode.Mouse1))
    {
      // yaw for looking side to side, pitch for looking up and down
      yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
      pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
      pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

      transform.eulerAngles = new Vector3(pitch, yaw);
    }

    transform.position = target.position - transform.forward * distFromTarget;
  }
}
