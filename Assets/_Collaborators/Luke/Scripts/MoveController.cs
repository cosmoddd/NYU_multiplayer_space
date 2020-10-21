using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityAtoms.BaseAtoms;

public class MoveController : NetworkBehaviour
{
    // high-level bool to check if we're in chat mode or not 


    public float turnAroundSpeed = 0.2f;
    public float walkSpeed = 20.0f;
    public float runSpeed = 40.0f;
    public bool bAllowJumping = true;
    float moveSpeed;

    public float gravity = 9.81f;
    public float jumpHeight = 10.0f;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;

    public GameObject cameraPrefab;

    CharacterController cc;
    Transform cameraTransform;

    float velocityY;
    public Vector2 inputDirection;
    Vector2 previousInputDir;
    
    [Header("In Chat Mode")]
    public BoolVariable inChatMode;
    //public GameObject Player;
    //public GameObject startPos;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // only spawn and assign camera if we are the owning player
        if(isLocalPlayer)
        {
            cc = GetComponent<CharacterController>();
            GameObject cameraObject = Instantiate(cameraPrefab, transform.position, transform.rotation);
            cameraObject.GetComponent<CameraController>().target = transform;
            cameraTransform = cameraObject.transform;


           // GameObject startPos = GameObject.Find("Startposition");

        }     
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer && inChatMode.Value == false)
        {
            // using GetAxis so that Gamepads will also be compatible
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            inputDirection.Normalize();

            bool isSprinting = Input.GetAxisRaw("Sprint") > 0;

            // function to handle actual movement
            Move(inputDirection, isSprinting);

            if (Input.GetAxisRaw("Jump") > 0)
            {
                Jump();
            }
        }       
    }

    void Move(Vector2 inputDir, bool isSprinting)
    {
        if(inputDir != Vector2.zero)
        {
            // rotate the character based on the user input direction plus the camera rotation
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

            // slow down speed when new input is opposite direction of our previous input
            if(Vector2.Dot(inputDir, previousInputDir) < 0.0f)
            {
                moveSpeed = turnAroundSpeed;
            }

            previousInputDir = inputDir;
        }

        // decide which speed to use based on bool parameter input
        float targetSpeed = (isSprinting ? runSpeed : walkSpeed) * inputDir.magnitude;
        moveSpeed = Mathf.SmoothDamp(moveSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        Vector3 slopeNormal;
        Vector3 forwardAngle = transform.forward;
        if (OnSlope(out slopeNormal))
        {
            // if we are on a slope find the slope angle and make that our forward vector
            forwardAngle = Vector3.Cross(slopeNormal, inputDir.magnitude * -transform.right);
        }

        Vector3 velocity = forwardAngle * moveSpeed;

        velocityY -= Time.deltaTime * gravity;

        // apply vertical direction based on Y velocity
        velocity += Vector3.up * velocityY;

        cc.Move(velocity * Time.deltaTime);

        // match current moveSpeed to CharacterController velocity
        moveSpeed = new Vector2(cc.velocity.x, cc.velocity.z).magnitude;

        // reset velocityY when we are on ground
        if (cc.isGrounded)
        {
            // set to -.85 to ensure character capsule is touching the ground
            velocityY = -0.85f;
        }
    }

    void Jump()
    {
        if(cc.isGrounded)
        {
            // kinematic equation to get jumpVelocity for desired height
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

    bool OnSlope(out Vector3 slopeNormal)
    {
        if(cc.isGrounded)
        {
            slopeNormal = Vector3.zero;
            return false;
        }

        RaycastHit Hit;
        if (Physics.Raycast(transform.position, Vector3.down, out Hit, (cc.height / 2) + 2.0f))
        {
            if (Hit.normal != Vector3.up)
            {
                slopeNormal = Hit.normal;
                return true;
            }
        }

        slopeNormal = Vector3.zero;
        return false;
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Playerkillzone"))
    //    {
    //        Destroy(gameObject);
    //        //NetworkManager.RegisterStartPosition(transform);
    //       // Instantiate(Player, startPos.transform.position, transform.rotation);
    //    }
    //}
}
