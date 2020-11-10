using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class UI_InputFieldLimiter : MonoBehaviour
{
    [Tooltip("The input field you want to limit.")]
    public TMP_InputField limitedInputField;

    [Tooltip("Max length of input field.")]
    public int characterLimit;

    void Start()
    {
        //Changes the character limit in the main input field.
        limitedInputField.characterLimit = characterLimit;
    }
}
