using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainCameraDisable : MonoBehaviour
{

    public GameObject loginText;

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
