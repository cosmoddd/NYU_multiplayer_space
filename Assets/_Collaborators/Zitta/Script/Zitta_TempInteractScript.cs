using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_TempInteractScript : MonoBehaviour {

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Process()
    {
        SendMessage("Interact");
        print("Interact");
    }
}
