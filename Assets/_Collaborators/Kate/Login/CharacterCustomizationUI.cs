using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* author: Kate Howell
*  This script controls the User Interface for the Character Customizer and Login Screen
*  public UI elements must be set in inspector
*/
public class CharacterCustomizationUI : MonoBehaviour
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
    public TextMeshProUGUI avatarIdTxt;
    public TextMeshProUGUI hatIdTxt;
    //--Sliders--
    public Slider[] sliders; //0-2 are avatar, 3-5 are hat
    //private vars
    CharacterCustomizerScript customizerScript;
     //--ColorValues--   these are used to make new colors when the sliders change
    Vector3 avatarColorValues;
    Vector3 hatColorValues;






    void Start()
    {
        customizerScript = this.GetComponent<CharacterCustomizerScript>();

        //start with empty colors
        avatarColorValues = new Vector3(0,0,0);
        hatColorValues = new Vector3(0,0,0);

        //set all sliders to 0 in beginning
        foreach(Slider slider in sliders)
        {
            slider.SetValueWithoutNotify(0);
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

    public void Enter()
    {
        //TODO - loads into game
    }

//------Mesh Display Management-----
    public void AvatarMeshUp()
    {
        customizerScript.activeAvatarID += 1;
        avatarIdTxt.SetText(customizerScript.activeAvatarID.ToString());
    }
     public void AvatarMeshDown()
    {
        customizerScript.activeAvatarID -= 1;
        avatarIdTxt.SetText(customizerScript.activeAvatarID.ToString());
    }
    public void HatMeshUp()
    {
        customizerScript.activeHatID += 1;
        hatIdTxt.SetText(customizerScript.activeHatID.ToString());
    }
     public void HatMeshDown()
    {
        customizerScript.activeHatID -= 1;
        hatIdTxt.SetText(customizerScript.activeHatID.ToString());
    }

//------Color Slider Management-----

/// <summary>
/// Sets body color on characterCustomizer
/// </summary>
/// <param name="color"></param>
    void AvatarColorChange(Vector3 color)
    {
        customizerScript.bodyColor = new Color(color.x, color.y, color.z, 1);
    }
//----Button Functions for Avatar Color Change---
    public void AvatarColorChangeRed(float value)
    {
        avatarColorValues.x = value;
        AvatarColorChange(avatarColorValues);
    }
    public void AvatarColorChangeGreen(float value)
    {
        avatarColorValues.y = value;
        AvatarColorChange(avatarColorValues);
    }
    public void AvatarColorChangeBlue(float value)
    {
        avatarColorValues.z = value;
        AvatarColorChange(avatarColorValues);
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



}
