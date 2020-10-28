using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateButton : MonoBehaviour
{

    public Button button;

    public float t = 1f;
    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.Alpha9))
        {
            t -= Time.deltaTime;
            if (t <= 0)
            {
                button.interactable = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.H) || Input.GetKeyUp(KeyCode.Alpha9))
        {
            t = 1f;
        }
    }
}
