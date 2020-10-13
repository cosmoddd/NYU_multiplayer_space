using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
/* author: Kate Howell
*  This script controls the User Interface for the Character Customizer and Login Screen
*  public UI elements must be set in inspector
*/
public class CharacterCustomizationUI : NetworkBehaviour
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
    public TextMeshProUGUI hatIdTxt;
    public TextMeshProUGUI headIdTxt;

    public TextMeshProUGUI torsoIdTxt;
    public TextMeshProUGUI feetIdTxt;
    //--Sliders--
    public Slider[] sliders; //0-2 are hat, 3-5 are head, 6-9 are torso, 10-12 are feet
    //private vars
    public CharacterCustomizerScript customizerScript;
     //--ColorValues--   these are used to make new colors when the sliders change
    
    Vector3 hatColorValues;
    Vector3 headColorValues;
    Vector3 torsoColorValues;
    Vector3 feetColorValues;

    public NetworkManager manager;

    void Start()
    {
        //customizerScript = this.GetComponent<CharacterCustomizerScript>();

        //start with empty colors
        hatColorValues = new Vector3(.5f, .5f, .5f);
        headColorValues = new Vector3(.5f, .5f, .5f);
        torsoColorValues = new Vector3(.5f, .5f, .5f);
        feetColorValues = new Vector3(.5f, .5f, .5f);

        //set all sliders to 0 in beginning
        foreach (Slider slider in sliders)
        {
            slider.SetValueWithoutNotify(.5f);
        }

        //start in login panel
        customizePanel.SetActive(false);
        loginPanel.SetActive(true);
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
    }

    public void EnterHost()
    {
        //SAVE PRESET
        customizerScript.SaveTraitsToScript();
        manager.StartHost();
        
    }

    public void EnterClient()
    {
        //SAVE PRESET
        customizerScript.SaveTraitsToScript();
        manager.StartClient();
        
    }

//------Mesh Display Management-----

    public void HatMeshUp()
    {
        customizerScript.HatIDIncrement(1);
        hatIdTxt.SetText(customizerScript.activeHatID.ToString());
    }
    public void HatMeshDown()
    {
        customizerScript.HatIDIncrement(-1);
        hatIdTxt.SetText(customizerScript.activeHatID.ToString());
    }

    public void HeadMeshUp()
    {
        customizerScript.HeadIDIncrement(1);
        headIdTxt.SetText(customizerScript.activeHeadID.ToString());
    }
    public void HeadMeshDown()
    {
        customizerScript.HeadIDIncrement(-1);
        headIdTxt.SetText(customizerScript.activeHeadID.ToString());
    }

    public void FeetMeshUp()
    {
        customizerScript.FootIDIncrement(1);
        feetIdTxt.SetText(customizerScript.activeFootID.ToString());
    }
    public void FeetMeshDown()
    {
        customizerScript.FootIDIncrement(-1);
        feetIdTxt.SetText(customizerScript.activeFootID.ToString());
    }

    public void TorsoMeshUp()
    {
        customizerScript.TorsoIDIncrement(1);
        torsoIdTxt.SetText(customizerScript.activeTorsoID.ToString());
    }
    public void TorsoMeshDown()
    {
        customizerScript.TorsoIDIncrement(-1);
        torsoIdTxt.SetText(customizerScript.activeTorsoID.ToString());
    }

//------Color Slider Management-----

/// <summary>
/// Sets body color on characterCustomizer
/// </summary>
/// <param name="color"></param>
    void HeadColorChange(Vector3 color)
    {
        customizerScript.headColor = new Color(color.x, color.y, color.z, 1);
    }
//----Button Functions for Head Color Change---
    public void HeadColorChangeRed(float value)
    {
        headColorValues.x = value;
        HeadColorChange(headColorValues);
    }
    public void HeadColorChangeGreen(float value)
    {
        headColorValues.y = value;
        HeadColorChange(headColorValues);
    }
    public void HeadColorChangeBlue(float value)
    {
        headColorValues.z = value;
        HeadColorChange(headColorValues);
    }

/// <summary>
/// Sets hat color
/// </summary>
/// <param name="color"></param>
    void HatColorChange(Vector3 color)
    {
        customizerScript.hatColor = new Color(color.x, color.y, color.z, 1);
    }
//----Button Functions for Hat Color Change---
    public void HatColorChangeRed(float value)
    {
        hatColorValues.x = value;
        HatColorChange(hatColorValues);
    }
    public void HatColorChangeGreen(float value)
    {
        hatColorValues.y = value;
        HatColorChange(hatColorValues);
    }
    public void HatColorChangeBlue(float value)
    {
        hatColorValues.z = value;
        HatColorChange(hatColorValues);
    }

    
/// <summary>
/// Sets feet color
/// </summary>
/// <param name="color"></param>
    void FeetColorChange(Vector3 color)
    {
        customizerScript.footColor = new Color(color.x, color.y, color.z, 1);
    }
//----Button Functions for FeetColor Change---
    public void FeetColorChangeRed(float value)
    {
        feetColorValues.x = value;
        FeetColorChange(feetColorValues);
    }
    public void FeetColorChangeGreen(float value)
    {
        feetColorValues.y = value;
        FeetColorChange(feetColorValues);
    }
    public void FeetColorChangeBlue(float value)
    {
        feetColorValues.z = value;
        FeetColorChange(feetColorValues);
    }

    /// <summary>
/// Sets torso color
/// </summary>
/// <param name="color"></param>
    void TorsoColorChange(Vector3 color)
    {
        customizerScript.bodyColor = new Color(color.x, color.y, color.z, 1);
    }
//----Button Functions for FeetColor Change---
    public void TorsoColorChangeRed(float value)
    {
        torsoColorValues.x = value;
        TorsoColorChange(torsoColorValues);
    }
    public void TorsoColorChangeGreen(float value)
    {
        torsoColorValues.y = value;
        TorsoColorChange(torsoColorValues);
    }
    public void TorsoColorChangeBlue(float value)
    {
        torsoColorValues.z = value;
        TorsoColorChange(torsoColorValues);
    }
}
