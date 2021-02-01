using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using TMPro;
using UnityEngine.UI;

public class ClientUpdateAvailable : MonoBehaviour
{

    public BoolVariable newUpdateRequired;
    public TextMeshProUGUI updateWarning;
    public Button getNewVersion; 

    void Start()
    {
        if (newUpdateRequired.Value == true)
        {
            updateWarning.enabled = true;
            getNewVersion.gameObject.SetActive(true);
        }
        if (newUpdateRequired.Value == false)
        {
            updateWarning.enabled = false;
            getNewVersion.gameObject.SetActive(false);
        }
    }

    public void GoToWebsite()
    {
        Application.OpenURL("https://cosmodddnyu.itch.io/nyu-diy-multiplayer-space");
    }

}
