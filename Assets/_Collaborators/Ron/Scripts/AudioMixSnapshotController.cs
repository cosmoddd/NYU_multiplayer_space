using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixSnapshotController : MonoBehaviour
{
    public AudioMixer MainMix;

    public AudioMixerSnapshot HallwaySnapshot;
    public AudioMixerSnapshot RoomSnapshot;

    public float transitionTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter()
    {
        RoomSnapshot.TransitionTo(transitionTime);
    }

    void OnTriggerExit()
    {
        HallwaySnapshot.TransitionTo(transitionTime);
    }
}
