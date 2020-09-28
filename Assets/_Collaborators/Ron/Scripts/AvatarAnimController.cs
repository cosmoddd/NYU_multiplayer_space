using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimController : MonoBehaviour
{
    public Animator animController;
    // Start is called before the first frame update
    void Start()
    {
        animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
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
            animController.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            animController.SetTrigger("wave");
        }
    }
}
