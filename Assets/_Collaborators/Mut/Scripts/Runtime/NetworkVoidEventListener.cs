using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using Mirror;

public class NetworkVoidEventListener : NetworkBehaviour
{
  public VoidEvent baseEvent;
  private VoidEvent localEvent;

  private void Awake()
  {
    if (isServer)
    {
      baseEvent.Register(() => RpcInvokeEventInClients());
    }
    localEvent = new VoidEvent();
  }

  public void Raise()
  {
    CmdInvokeServerEvent();
  }

  [Command]
  void CmdInvokeServerEvent()
  {

  }

  [ClientRpc]
  void RpcInvokeEventInClients()
  {
    baseEvent.Raise();
  }
}