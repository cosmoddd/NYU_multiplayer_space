using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;
using UnityAtoms.BaseAtoms;

public class OptionsSave : MonoBehaviour
{    
    public OptionsMenu localMenu;

    public void LoadSave()
    {
        if (ES3.FileExists("userSettings.es3"))
        {
            LoadData();
        }
    }

    void LoadData()
    {
        float localMouseSense = ES3.Load("mouseSensitivity", "userSettings.es3", 1.0f);
        float localVolume = ES3.Load("volume", "userSettings.es3", 1.0f);

        //TODO consider change to a size 2 array of int
        // saves memory?
        Vector2 local_Resolution = ES3.Load("resolution", "userSettings.es3", new Vector2(Screen.width, Screen.height));

        localMenu.ChangeMouseSense(localMouseSense);
        localMenu.AdjustMasterVolume(localVolume);
        localMenu.GetCamControl().bInvertY.SetValue(ES3.Load("invertMouse", "userSettings.es3", false));
        Screen.fullScreen = ES3.Load("fullscreen", "userSettings.es3", true);
        localMenu.SetResolution(local_Resolution);
    }

    public void SaveData()
    {
        ES3.Save("mouseSensitivity", localMenu.GetCamControl().mouseSensitivity, "userSettings.es3");
        ES3.Save("volume", AudioListener.volume, "userSettings.es3");
        ES3.Save("invertMouse", localMenu.GetCamControl().bInvertY.Value, "userSettings.es3");
        ES3.Save("fullscreen", Screen.fullScreen, "userSettings.es3");
        ES3.Save("resolution", new Vector2(Screen.currentResolution.width, Screen.currentResolution.height), "userSettings.es3");
    }
}
