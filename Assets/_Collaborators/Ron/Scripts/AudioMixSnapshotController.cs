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


    void OnTriggerEnter(Collider other)
    {
      // print(other.name+" entered the mix joint.");

        if (other.GetComponent<NetworkBehaviour>() && other.GetComponent<NetworkBehaviour>().isLocalPlayer)
        {
          RoomSnapshot.TransitionTo(transitionTime);
        }
    }

    void OnTriggerExit(Collider other)
    {
      // print(other.name+" left the mix joint.");

      if (other.GetComponent<NetworkBehaviour>() && other.GetComponent<NetworkBehaviour>().isLocalPlayer)
      {
        HallwaySnapshot.TransitionTo(transitionTime);
      }
    }
}
