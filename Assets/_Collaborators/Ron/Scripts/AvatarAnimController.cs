using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 
public class AvatarAnimController : NetworkBehaviour

{
    public Animator animController;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            animController = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                animController.SetBool("isWalking", true);
            }
            else
            {
                animController.SetBool("isWalking", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                animController.SetBool("jump", true);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                animController.SetBool("jump", false);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                animController.SetBool("wave", true);
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                animController.SetBool("wave", false);
            }

        }
    }
}
