using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NYUMultiplayerSpace
{
  [RequireComponent(typeof(NetworkManager))]
  public class LobbyManagerPublicMethods : MonoBehaviour
  {
    private NetworkManager manager;

    private void Awake()
    {
      manager = GetComponent<NetworkManager>();
    }

    public void StartClient()
    {
      manager.StartClient();
    }

    public void StartHost()
    {
      manager.StartHost();
    }

    public void StartClient(string ip)
    {
      manager.networkAddress = ip;
      StartClient();
    }
  }
}