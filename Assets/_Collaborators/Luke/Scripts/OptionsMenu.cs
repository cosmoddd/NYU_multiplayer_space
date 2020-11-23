using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Slider mouseSenseSlider;
    public NetworkManagerGC manager;
    public GameObject mainPanel;
    public GameObject confirmQuitPanel;
    public GameObject mouseSettingPanel;
    public GameObject videoSettingPanel;

    public Text mouseSenseText;
    CameraController camController;

    Resolution[] resolutions;

    private void Awake()
    {
        manager = FindObjectOfType<NetworkManagerGC>();
    }

    public void Initialize(CameraController controller)
    {
        camController = controller;
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            options.Add(option);   
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        mouseSenseSlider.value = camController.mouseSensitivity;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void LeaveSession()
    {
        manager.StopClient();
        Application.Quit();
    }

    public void ConfirmQuit()
    {
        confirmQuitPanel.SetActive(true);
    }

    public void CancelQuit()
    {
        confirmQuitPanel.SetActive(false);
    }

    public void ShowMousePanel()
    {
        mouseSettingPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void HideMousePanel()
    {
        mouseSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowVideoPanel()
    {
        videoSettingPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void HideVideoPanel()
    {
        videoSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        gameObject.SetActive(false);
        mouseSettingPanel.SetActive(false);
        videoSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ChangeMouseSense(float newSense)
    {
        camController.mouseSensitivity = newSense;

        // also update mouse sense text in menu
        mouseSenseText.text = newSense.ToString("G2");
    }

}
