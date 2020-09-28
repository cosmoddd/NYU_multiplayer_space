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
      // ////currentServerPosition = movieTheatre.
      // /if (Input.GetKeyDown(KeyCode.Alpha1))
      // {
      //   currentServerPosition = (0.25f);
      // }
      // if (Input.GetKeyDown(KeyCode.Alpha1))
      // {
      //   currentServerPosition = (0.50f);
      // }
      // if (Input.GetKeyDown(KeyCode.Alpha2))
      // {
      //   currentServerPosition = (0.75f);
      // }
    }

    void ChangeCurrentServerPosition(float oldValue, float newValue)
    {
      print(newValue);
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
      else
      {
        CmdPlay();
      }
    }

    public void Pause()
    {
      if (isServer)
      {
        RpcPause();
      }
      else
      {
        CmdPause();
      }
    }

    public void Stop()
    {
      if (isServer)
      {
        RpcStop();
        currentServerPosition = 0;
      }
      else
      {
        CmdStop();
      }

    }

    [Command]
    void CmdPlay()
    {
      RpcPlay();
    }

    [Command]
    void CmdPause()
    {
      RpcPause();
    }

    [Command]
    void CmdStop()
    {
      RpcStop();
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
    void RpcStop()
    {
      movieTheatre.Stop();
    }

    [ClientRpc]
    void RpcChangeVideoPosition(float newPosition)
    {
      movieTheatre.SetVideoPosition(newPosition);
    }
  }
}