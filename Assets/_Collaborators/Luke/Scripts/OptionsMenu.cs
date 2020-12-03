﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityAtoms.BaseAtoms;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Slider mouseSenseSlider;
    public Slider masterVolumeSlider;
    public NetworkManagerGC manager;
    public GameObject mainPanel;
    public GameObject confirmQuitPanel;
    public GameObject mouseSettingPanel;
    public GameObject videoSettingPanel;
    public GameObject audioSettingPanel;

    public Text mouseSenseText;
    public Text volumeText;
    CameraController camController;

    [Header("In Chat Mode")]
    public BoolVariable inChatMode;

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
        int initialScreenIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if(Screen.currentResolution.width == resolutions[i].width
                && Screen.currentResolution.height == resolutions[i].height)
            {
                initialScreenIndex = i;
            }

            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = initialScreenIndex;
        resolutionDropdown.RefreshShownValue();

        masterVolumeSlider.maxValue = AudioListener.volume;
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

    public void ShowAudioPanel()
    {
        audioSettingPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void HideAudioPanel()
    {
        audioSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        gameObject.SetActive(false);
        mouseSettingPanel.SetActive(false);
        videoSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
        inChatMode.SetValue(false);
    }

    public void ChangeMouseSense(float newSense)
    {
        camController.mouseSensitivity = newSense;

        // also update mouse sense text in menu
        mouseSenseText.text = newSense.ToString("G2");
    }

    public void AdjustMasterVolume(float newVolume)
    {
        AudioListener.volume = newVolume;

        int textVal = (int)((newVolume / masterVolumeSlider.maxValue) * 100.0f);
        volumeText.text = textVal.ToString("D");
    }

}
