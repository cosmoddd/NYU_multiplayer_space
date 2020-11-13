using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LevelLoading : NetworkBehaviour
{

    public Object thisScene;

    void OnTriggerEnter()
    {
        if(isClient)
        {
            {
                SceneManager.LoadSceneAsync(thisScene.name, LoadSceneMode.Additive);
            }
            
            // SceneMessage msg = new SceneMessage
            // {
            //     sceneName = thisScene.name,
            //     sceneOperation = SceneOperation.LoadAdditive
            // };

            // connectionToClient.Send(msg);
        }
    }

    

    void OnTriggerExit()
    {
        // if (isClient)
        {
            SceneManager.UnloadSceneAsync(thisScene.name);
        }
    }

}
