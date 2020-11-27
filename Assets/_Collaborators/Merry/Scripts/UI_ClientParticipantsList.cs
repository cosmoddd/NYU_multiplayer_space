using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Mirror;

public class UI_ClientParticipantsList : NetworkBehaviour
{
    public TextMeshProUGUI localParticipantsList = null;
    public string playerString, clientName;
    public static event Action<string> ClientLeft;

    public override void OnStartClient()
    {
        base.OnStartClient();

        clientName = GetComponent<MeshAssigner>().userName;
    }

    private void OnDestroy() //when unity destroys this object (here, when the player logs off)
    {
        ClientLeft?.Invoke(clientName);
    }

    void ClientList(List<string> playerList)
    {
        playerString = ("Current Users:\n ");
        Debug.Log("Message recieved");

        foreach(string x in playerList) //for each string in the list
        {
            playerString += (" - " + x + "\n");
        }

        localParticipantsList.text = playerString;
    }

    private void OnEnable()
    {
        UI_ParticipantsList.UsersUpdated += ClientList;
    }

    private void OnDisable()
    {
        UI_ParticipantsList.UsersUpdated -= ClientList;
    }


}
