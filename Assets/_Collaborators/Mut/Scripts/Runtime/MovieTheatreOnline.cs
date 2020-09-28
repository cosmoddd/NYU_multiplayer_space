using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NYUMultiplayerSpace
{
  [RequireComponent(typeof(MovieTheatre))]
  public class MovieTheatreOnline : NetworkBehaviour
  {
    private MovieTheatre movieTheatre;

    [SyncVar(hook = nameof(ChangeCurrentServerPosition))]
    public float currentServerPosition;

    // Start is called before the first frame update
    void Start()
    {
      movieTheatre = GetComponent<MovieTheatre>();
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Exclaim))
      {
        currentServerPosition = (0.25f);

      }
      if (Input.GetKeyDown(KeyCode.At))
      {
        currentServerPosition = (0.50f);

      }
      if (Input.GetKeyDown(KeyCode.Hash))
      {
        currentServerPosition = (0.75f);
      }
    }

    void ChangeCurrentServerPosition(float oldValue, float newValue)
    {
      if (isServer)
      {
        RpcChangeVideoPosition(newValue);
      }
    }

    public void Play()
    {
      if (isServer)
      {
        RpcPlay();
      }
      movieTheatre.Play();
    }

    public void Pause()
    {
      if (isServer)
      {
        RpcPlay();
      }
      movieTheatre.Pause();
    }

    public void Stop()
    {
      if (isServer)
      {
        RpcPause();
      }

      movieTheatre.Pause();
      currentServerPosition = 0;
    }

    [ClientRpc]
    void RpcPause()
    {
      movieTheatre.Pause();
    }

    [ClientRpc]
    void RpcPlay()
    {
      movieTheatre.Play();
    }

    [ClientRpc]
    void RpcChangeVideoPosition(float newPosition)
    {
      movieTheatre.SetVideoPosition(newPosition);
    }
  }
}