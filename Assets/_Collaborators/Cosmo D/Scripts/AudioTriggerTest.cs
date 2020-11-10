using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTriggerTest : MonoBehaviour
{
    public AudioMixer thisMixer;

    public AudioMixerSnapshot mainSnapshot;
    public AudioMixerSnapshot targetSnapshot;
    public float transitionTime;

    void OnTriggerEnter()
    {
        targetSnapshot.TransitionTo(transitionTime);
    }

    void OnTriggerExit()
    {
        mainSnapshot.TransitionTo(transitionTime);
    }
}
