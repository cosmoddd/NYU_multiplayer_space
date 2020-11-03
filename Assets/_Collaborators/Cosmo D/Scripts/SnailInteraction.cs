using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SnailInteraction : NetworkBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [Command(ignoreAuthority = true)]
    public void CmdSnailJump() => RpcSnailJump();

    [ClientRpc]
    public void RpcSnailJump()
    {
        print("Jump, snal!");
        animator.SetBool("jump", true);
        StartCoroutine(ParameterFalse("jump"));
    }

    IEnumerator ParameterFalse(string perameter)
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool(perameter, false);
    }
}
