﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLoginSpace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync("Login Scene", LoadSceneMode.Single);        
    }


}
