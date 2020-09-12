﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float walkSpeed = 20.0f;
    public float runSpeed = 40.0f;
    float moveSpeed;

    public float gravity = 9.81f;
    public float jumpHeight = 10.0f;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    CharacterController cc;
    Transform cameraTransform;

    float velocityY;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // using GetAxis so that Gamepads will also be compatible
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDirection.Normalize();

        bool isSprinting = Input.GetAxisRaw("Sprint") > 0;

        // function to handle actual movement
        Move(inputDirection, isSprinting);

        if(Input.GetAxisRaw("Jump") > 0)
        {
            Jump();
        }
    }

    void Move(Vector2 inputDir, bool isSprinting)
    {
        if(inputDir != Vector2.zero)
        {
            // rotate the character based on the user input direction plus the camera rotation
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        // decide which speed to use based on bool parameter input
        moveSpeed = (isSprinting ? runSpeed : walkSpeed) * inputDir.magnitude;

        Vector3 velocity = transform.forward * moveSpeed; 
        velocityY -= Time.deltaTime * gravity;

        // apply vertical direction based on Y velocity
        velocity += Vector3.up * velocityY;

        cc.Move(velocity * Time.deltaTime);

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
}
