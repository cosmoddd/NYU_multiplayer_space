using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityAtoms.BaseAtoms;

public class Z_Interactee : NetworkBehaviour {

    public UnityEvent onPressed;

    public void Process()
    {
        // SendMessage("Interact");
        print("Interact");

        onPressed.Invoke();
    }

}
