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
    public GameObject clientLoginPanel;

    //--InputFields--

    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    public TMP_InputField displayName;

    public Sprite[] headSprites;
    public Sprite[] torsoSprites;
    public Sprite[] feetSprites;

    public Sprite[] hatSprites;

    public Image[] optionImages;

    public GameObject colorPicker;

    public int currentTab; //0 head, 1 torso, 2 is feet, 3 is hat, 4 is acces

    public AuthenticationManager authentication;

    public Slider[] torsoSliders;

    public GameObject slidersPanel;

    public GameObject scrollPanel;


    public Customizer customizerScript;

    public MeshAssigner meshAssigner;
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

        if(loginPanel)
        {
            loginPanel.SetActive(true);
        }
        
        if(colorPicker)
        {
            colorPicker.gameObject.SetActive(false);
        }
        
        if(clientLoginPanel)
        {
            clientLoginPanel.SetActive(false);
        }
    }



    /// <summary>
    ///  when "Go" is pressed, this function sets the character name to the user name
    /// </summary>
    public void Login()
    {
        if(emailField == null) Debug.Log("no emial feild");
        if(passwordField == null) Debug.Log("no password feild");
        //print("username: " + username);
        //customizerScript.userName = email;
         bool login = authentication.Login(emailField.text, passwordField.text);
        if(!login)
        {
            //what to do if a login fails, let them try again
            print("login failed");
            return;
        }

        //save login info
        customizerScript.loginInfo = authentication.GetLoginInfo(emailField.text);
        

        //back button

        customizePanel.SetActive(true);
        slidersPanel.SetActive(true);
        scrollPanel.SetActive(false);
        loginPanel.SetActive(false);
        SetSliderValues();

        SetCustomizationTab(currentTab);
        colorPicker.gameObject.SetActive(true);
    }

    void SetSliderValues()
    {
        for(int i = 0; i < torsoSliders.Length; i++)
        {
            float clampDif = customizerScript.torsoScaleClamps[i].y - customizerScript.torsoScaleClamps[i].x; //the range between the two clamp values
            float currentValue = customizerScript.torsoNodeScales[i]; //the current scale of that node, set by the load data script
            currentValue = currentValue - customizerScript.torsoScaleClamps[i].x;

            float percentageValue = (currentValue) / clampDif; //the percentage the current value is, from 0-1
           // float percentageValue = ((currentValue - clampDif) * clampDif) + 1;

            //float currentValue = clampDif  +  ((value - 1) * clampDif); //the real value from the percentage value
            //current value - clampdiff = (value - 1) / clamp diff
            //(current value - clampdiff ) * clamp diff = value - 1

            Debug.Log(percentageValue);
            torsoSliders[i].SetValueWithoutNotify(percentageValue); //set the torso sliders inital value to this value
        }
    }

        public void SaveCharacter()
    {
        //change tabs
        customizePanel.SetActive(false);
        colorPicker.gameObject.SetActive(false);

        clientLoginPanel.SetActive(true);

        //SET USERNAME HERE?
        if(customizerScript.userName != "") //if the player has a username, set it in the text feild
        {
            displayName.SetTextWithoutNotify(customizerScript.userName);
        }

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

    public void NetworkPickOption(int optionID)
    {
        switch (currentTab)
        {
            case 0:
                meshAssigner.CmdChangeTraitID(1, optionID);
                break;
            case 1:
                meshAssigner.CmdChangeTraitID(4, optionID);
                break;
            case 2:
                meshAssigner.CmdChangeTraitID(2, optionID);
                meshAssigner.CmdChangeTraitID(3, optionID);
                break;
            case 3:
                meshAssigner.CmdChangeTraitID(0, optionID);
                break;
        }
    }

    public void ShowCustomizePanel()
    {
        if(customizePanel)
        {
            customizePanel.SetActive(true);
        }
    }

    public void HideCustomizePanel()
    {
        if (customizePanel)
        {
            customizePanel.SetActive(false);
        }
    }

    public void SkipLogin()
    {
        customizePanel.SetActive(true);
        SetCustomizationTab(0);
    }

    //Tab BUTTONS
    public void SelectHeadTab()
    {
        SetCustomizationTab(0);

        if(slidersPanel)
            slidersPanel.SetActive(false);

        scrollPanel.SetActive(true);
        
    }

    public void SelectTorsoTab()
    {
        SetCustomizationTab(1);

        if (slidersPanel)
            slidersPanel.SetActive(true);

        scrollPanel.SetActive(false);
    }

    public void SelectFeetTab()
    {
        SetCustomizationTab(2);

        if (slidersPanel)
            slidersPanel.SetActive(false);

        scrollPanel.SetActive(true);
    }

    public void SelectHatTab()
    {
        SetCustomizationTab(3);

        if (slidersPanel)
            slidersPanel.SetActive(false);

        scrollPanel.SetActive(true);
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

    
    public void SetDisplayName(string input)
    {
        string username = input;
        customizerScript.userName = username;
    }

    void SliderSetValue(float value, int slider)
    {
        float clampDif = customizerScript.torsoScaleClamps[slider].y - customizerScript.torsoScaleClamps[slider].x; //the range between the two clamp values

        float currentValue = clampDif  +  ((value - 1) * clampDif); //the real value from the percentage value

        customizerScript.torsoNodeScales[slider] = currentValue + customizerScript.torsoScaleClamps[slider].x;

    }

    public void BackButton()
    {
      customizePanel.SetActive(true);
        colorPicker.gameObject.SetActive(true);
        customizePanel.SetActive(true);
        slidersPanel.SetActive(true);
        scrollPanel.SetActive(false);
        SetSliderValues();

        SetCustomizationTab(currentTab);

        clientLoginPanel.SetActive(false);

    }

    public void SliderZeroSetValue(float value){  SliderSetValue(value,0); }
    public void SliderOneSetValue(float value){  SliderSetValue(value,1); }
    public void SliderTwoSetValue(float value){  SliderSetValue(value,2); }
    public void SliderThreeSetValue(float value){  SliderSetValue(value,3); }
    public void SliderFourSetValue(float value){  SliderSetValue(value,4); }
    public void SliderFiveSetValue(float value){  SliderSetValue(value,5); }
    public void SliderSixSetValue(float value){  SliderSetValue(value,6); }
    public void SliderSevenSetValue(float value){  SliderSetValue(value,7); }


}
