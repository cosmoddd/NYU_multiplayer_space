using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizerUITabs : MonoBehaviour
{
    //public UI elements to be set in inspector
    //---Panels--
    public GameObject loginPanel;
    public GameObject customizePanel;
    //--InputFields--
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    //--Text--
    public TextMeshProUGUI usernameTxt;

    public Sprite[] headSprites;
    public Sprite[] torsoSprites;
    public Sprite[] feetSprites;

    public Sprite[] hatSprites;

    public Image[] optionImages;

    public GameObject colorPicker;

    //public GameObject hatTab;
    //public GameObject headTab;
    //public GameObject torsoTab;
    //public GameObject feetTab;

    public int currentTab; //0 head, 1 torso, 2 is feet, 3 is hat, 4 is acces


    public Customizer customizerScript;
    //--ColorValues--   these are used to make new colors when the sliders change

    Vector3 hatColorValues;
    Vector3 headColorValues;
    Vector3 torsoColorValues;
    Vector3 feetColorValues;

    void Start()
    {
        // customizerScript = this.GetComponent<CharacterCustomizerScript>();

        //start with empty colors
        hatColorValues = new Vector3(.5f, .5f, .5f);
        headColorValues = new Vector3(.5f, .5f, .5f);
        torsoColorValues = new Vector3(.5f, .5f, .5f);
        feetColorValues = new Vector3(.5f, .5f, .5f);


        //start in login panel
        customizePanel.SetActive(false);
        loginPanel.SetActive(true);
        colorPicker.gameObject.SetActive(false);
    }

    /// <summary>
    ///  when "Go" is pressed, this function sets the character name to the user name
    ///   TODO - add password functionality
    /// </summary>
    public void Login()
    {
        string username = usernameField.text;
        //print("username: " + username);
        customizerScript.characterName = username;

        customizePanel.SetActive(true);
        loginPanel.SetActive(false);

        usernameTxt.SetText(username);
        SetCustomizationTab(currentTab);
        colorPicker.gameObject.SetActive(true);
    }

    // public void Enter()
    // {
    //SAVE PRESET
    //customizerScript.SaveTraitsToScript();
    //TODO - loads into game
    // }

    public void EnterHost()
    {
        //SAVE PRESET
        customizerScript.SaveData();
        customizerScript.manager.StartHost();
    }

    public void EnterClient()
    {
        //SAVE PRESET
        customizerScript.SaveData();
        customizerScript.manager.StartClient();

    }

    public void SetCustomizationTab(int tabID)
    {
        currentTab = tabID;
        SetImageOptions(tabID);
    }

    Sprite[] imageOptions;

    void SetImageOptions(int tabID)
    {
        // Sprite[] imageOptions;
        GetCurrentImages(out imageOptions, tabID);
        int c = 0;
        foreach (Image im in optionImages)
        {
            if (c >= imageOptions.Length)
            {
                im.gameObject.SetActive(false);
            }
            else
            {
                im.gameObject.SetActive(true);
                im.sprite = imageOptions[c];
            }

            c++;
        }
    }

    void GetCurrentImages(out Sprite[] imageOptions, int tabID)
    {
        print("Get some images!");
        imageOptions = headSprites;
        switch (tabID)
        {
            case 1:
                imageOptions = torsoSprites;
                break;
            case 2:
                imageOptions = feetSprites;
                break;
            case 3:
                imageOptions = hatSprites;
                break;
        }
    }

    public void PickOption(int optionID)
    {
        switch (currentTab)
        {
            case 0:
                customizerScript.bodyIDs[1] = optionID;
                break;
            case 1:
                customizerScript.bodyIDs[4] = optionID;
                break;
            case 2:
                customizerScript.bodyIDs[2] = optionID;
                customizerScript.bodyIDs[3] = optionID;
                break;
            case 3:
                customizerScript.bodyIDs[0] = optionID;
                break;
        }
    }

    //Tab BUTTONS
    public void SelectHeadTab()
    {
        SetCustomizationTab(0);
    }

    public void SelectTorsoTab()
    {
        SetCustomizationTab(1);
    }

    public void SelectFeetTab()
    {
        SetCustomizationTab(2);
    }

    public void SelectHatTab()
    {
        SetCustomizationTab(3);
    }

    //COLOR
    public void SetColor(Color selectedColor)
    {
        switch (currentTab)
        {
            case 0:
                customizerScript.bodyColors[1] = selectedColor;
                break;
            case 1:
                customizerScript.bodyColors[4] = selectedColor;
                break;
            case 2:
                customizerScript.bodyColors[2] = selectedColor;
                customizerScript.bodyColors[3] = selectedColor;
                break;
            case 3:
                customizerScript.bodyColors[0] = selectedColor;
                break;
        }
    }
}
