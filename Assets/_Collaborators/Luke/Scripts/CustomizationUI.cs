using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* author: Kate Howell
*  This script controls the User Interface for the Character Customizer and Login Screen
*  public UI elements must be set in inspector
*/
public class CustomizationUI : MonoBehaviour
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
    public Customizer customizerScript;
    //--ColorValues--   these are used to make new colors when the sliders change

    Vector3 hatColorValues;
    Vector3 headColorValues;
    Vector3 torsoColorValues;
    Vector3 feetColorValues;

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

    void Update()
    {
        hatColorValues = new Vector3 (sliders[0].value, sliders[1].value, sliders[2].value);
        headColorValues = new Vector3(sliders[3].value, sliders[4].value, sliders[5].value);
        feetColorValues = new Vector3(sliders[9].value, sliders[10].value, sliders[11].value);
        torsoColorValues = new Vector3(sliders[6].value, sliders[7].value, sliders[8].value);

        HatColorChange(hatColorValues);
        HeadColorChange(headColorValues);
        FeetColorChange(feetColorValues);
        TorsoColorChange(torsoColorValues);
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
        //SAVE PRESET
        //customizerScript.SaveTraitsToScript();
        //TODO - loads into game
    }

    //------Mesh Display Management-----

    public void HatMeshUp()
    {
        customizerScript.bodyIDChange(0, 1);
        hatIdTxt.SetText(customizerScript.bodyIDs[0].ToString());
    }
    public void HatMeshDown()
    {
        customizerScript.bodyIDChange(0, -1);
        hatIdTxt.SetText(customizerScript.bodyIDs[0].ToString());
    }

    public void HeadMeshUp()
    {
        customizerScript.bodyIDChange(1, 1);
        headIdTxt.SetText(customizerScript.bodyIDs[1].ToString());
    }
    public void HeadMeshDown()
    {
        customizerScript.bodyIDChange(1, -1);
        headIdTxt.SetText(customizerScript.bodyIDs[1].ToString());
    }

    public void FeetMeshUp()
    {
        customizerScript.bodyIDChange(2, 1);
        customizerScript.bodyIDChange(3, 1);
        feetIdTxt.SetText(customizerScript.bodyIDs[2].ToString());
    }
    public void FeetMeshDown()
    {
        customizerScript.bodyIDChange(2, -1);
        customizerScript.bodyIDChange(3, -1);
        feetIdTxt.SetText(customizerScript.bodyIDs[2].ToString());
    }

    public void TorsoMeshUp()
    {
        customizerScript.bodyIDChange(4, 1);
        torsoIdTxt.SetText(customizerScript.bodyIDs[4].ToString());
    }
    public void TorsoMeshDown()
    {
        customizerScript.bodyIDChange(4, -1);
        torsoIdTxt.SetText(customizerScript.bodyIDs[4].ToString());
    }

    //------Color Slider Management-----

    /// <summary>
    /// Sets body color on characterCustomizer
    /// </summary>
    /// <param name="color"></param>
    void HeadColorChange(Vector3 color)
    {
        customizerScript.bodyColors[1] = new Color(color.x, color.y, color.z, 1);
    }

    /// <summary>
    /// Sets hat color
    /// </summary>
    /// <param name="color"></param>
    void HatColorChange(Vector3 color)
    {
        customizerScript.bodyColors[0] = new Color(color.x, color.y, color.z, 1);
    }

    /// <summary>
    /// Sets feet color
    /// </summary>
    /// <param name="color"></param>
    void FeetColorChange(Vector3 color)
    {
        customizerScript.bodyColors[2] = new Color(color.x, color.y, color.z, 1);
        customizerScript.bodyColors[3] = new Color(color.x, color.y, color.z, 1);
    }

    /// <summary>
    /// Sets torso color
    /// </summary>
    /// <param name="color"></param>
    void TorsoColorChange(Vector3 color)
    {
        customizerScript.bodyColors[4] = new Color(color.x, color.y, color.z, 1);
    }
}
