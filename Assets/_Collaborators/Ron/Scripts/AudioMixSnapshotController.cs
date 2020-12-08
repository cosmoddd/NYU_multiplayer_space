using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Mirror;

public class AudioMixSnapshotController : NetworkBehaviour
{
    public AudioMixer MainMix;

    public AudioMixerSnapshot HallwaySnapshot;
    public AudioMixerSnapshot RoomSnapshot;

    public float transitionTime;


    void OnTriggerEnter()
    {
        if (isLocalPlayer)
        {
          RoomSnapshot.TransitionTo(transitionTime);
        }
    }

    void OnTriggerExit()
    {
      if (isLocalPlayer)
      {
        HallwaySnapshot.TransitionTo(transitionTime);
      }
    }
}
