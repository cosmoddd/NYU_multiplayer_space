using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zitta_TempInteractScript : MonoBehaviour {

    public UnityEvent onPressed;

    public void Process()
    {
        SendMessage("Interact");
        print("Interact");

        onPressed.Invoke();
    }



}
