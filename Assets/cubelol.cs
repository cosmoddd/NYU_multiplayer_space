using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class cubelol : MonoBehaviour
{

    public struct userAttributes { }
    public struct appAttributes { }


    public bool isBlue = false;


    public Material blueMaterial;
    public Material redMaterial;

    public Renderer rend;

    // Start is called before the first frame update
    void Awake()
    {
        ConfigManager.FetchCompleted += SetColor;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes { }, new appAttributes { });
    }

    void SetColor(ConfigResponse response)
    {
        isBlue = ConfigManager.appConfig.GetBool("cubeIsBlue");

        if (isBlue)
        {
            rend.material = blueMaterial;

        }
        else
        {
            rend.material = redMaterial;
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
