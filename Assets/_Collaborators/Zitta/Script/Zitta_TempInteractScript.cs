using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Zitta_TempInteractScript : NetworkBehaviour {

    public UnityEvent onPressed;

    public void Process()
    {
        SendMessage("Interact");
        print("Interact");

        onPressed.Invoke();
    }



}
