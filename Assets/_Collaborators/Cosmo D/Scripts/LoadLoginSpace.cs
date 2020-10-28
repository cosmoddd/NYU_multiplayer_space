using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLoginSpace : MonoBehaviour
{
    public string loginScene = "Main Login 2";

    void Start()
    {
        SceneManager.LoadSceneAsync(loginScene, LoadSceneMode.Single);        
    }


}
