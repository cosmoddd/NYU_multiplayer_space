using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using TMPro;

public class MainCameraDisable : NetworkBehaviour
{

    public GameObject loginText;
    public TextMeshPro loginTextTMP;

    public void Start()
    {
        if (isServerOnly)
        {
          GetComponent<AudioListener>().enabled= true;
          loginTextTMP.text = "YOU are the SERVER!";
        }
        else
        {
          print("you aint the server!");
        }
    }

    void OnEnable()
    {
        ChatBehaviour.LoggedIn += DisableLoginText; 
    }

    void OnDisable()
    {
        ChatBehaviour.LoggedIn -= DisableLoginText; 
    }

    void DisableLoginText()
    {
        loginText.SetActive(false);
    }

}
