using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;

public class OptionsSave : MonoBehaviour
{
    float local_mouseSensitivity;
    float local_volume;
    bool local_isFullscreen;
    bool local_bInvertMouse;

    //TODO change to a size 2 array of int
    // saves memory
    Vector2 local_Resolution;

    public OptionsMenu localMenu;

    // Start is called before the first frame update
    void Start()
    {
        if(ES3.FileExists("userSettings.es3"))
        {
            LoadData();
        }
    }

    void LoadData()
    {
        localMenu.ChangeMouseSense(ES3.Load("mouseSensitivity", "userSettings.es3", 1.0f));
        localMenu.AdjustMasterVolume(ES3.Load("volume", "userSettings.es3", 1.0f));
        localMenu.camController.bInvertY.SetValue(ES3.Load("invertMouse", "userSettings.es3", false));
        Screen.fullScreen = ES3.Load("fullscreen", "userSettings.es3", true);
        local_Resolution = ES3.Load("resolution", "userSettings.es3", new Vector2(Screen.width, Screen.height));
        localMenu.SetResolution(local_Resolution);
        localMenu.SetSliders();
    }

    public void SaveData()
    {
        ES3.Save("mouseSensitivity", localMenu.camController.mouseSensitivity, "userSettings.es3");
        ES3.Save("volume", AudioListener.volume, "userSettings.es3");
        ES3.Save("invertMouse", localMenu.camController.bInvertY, "userSettings.es3");
        ES3.Save("fullscreen", Screen.fullScreen, "userSettings.es3");
        ES3.Save("resolution", new Vector2(Screen.currentResolution.width, Screen.currentResolution.height), "userSettings.es3");
    }
}
