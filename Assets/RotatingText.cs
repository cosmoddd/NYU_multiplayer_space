using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class RotatingText : MonoBehaviour
{
    public StringVariable FullText;
    public StringReference RepetitionDivider;

    [Header("Control Parameters")]
    public IntReference DisplayLength;
    public FloatReference TimeForNewLetter;
    public StringVariable PartialText;

    private string safeFullText => String.IsNullOrWhiteSpace(FullText.Value) ? "loading" : FullText.Value;
    private string repeatString => $"{safeFullText}{RepetitionDivider.Value}";
    public string currentSubstring =>
        $"{repeatString}{repeatString}{repeatString}".Substring(
                currentIndex,
                DisplayLength.Value);


    private int currentIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        FullText.Changed.Register(_ =>
        {
            currentIndex = 0;
            PartialText.Value = currentSubstring;
        });
    }

    // Update is called once per frame
    private float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if (FullText.Value.Length > 0 && timer > TimeForNewLetter.Value)
        {
            PartialText.Value = currentSubstring;
            currentIndex = (currentIndex + 1) % FullText.Value.Length;
            timer = 0;
        }
    }
}
