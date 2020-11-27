using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class ParticipantsReceiver : NetworkBehaviour
{
    public TextMeshProUGUI tmProtext;
    public string thisUserName;

    [TextArea(2,6)]
    public string userListString;

    public static event Action<string> LeavingServer;

    public static event Action<string> RemoveUser;

    public override void OnStartClient()
    {
        thisUserName = GetComponent<MeshAssigner>().userName;
    }

    public void SendUsersToList(List<string> thisList)
    {
        userListString = "<b>Users:</b> \n";
        foreach (string s in thisList)
        {
            userListString += (s+"\n");
        }
        tmProtext.text = userListString;
    }

    public void OnDestroy()
    {
        print("I'm outta here.");
        LeavingServer?.Invoke(thisUserName);
    }
    

    void UserLeft(string user)
    {
        if (isClient)
        {
            print("attempting to remove "+user);
            RemoveUser?.Invoke(user);
        }
    }
  
    void OnEnable()
    {
        ParticipantsTest.SendUsersToList += SendUsersToList;

        ParticipantsReceiver.LeavingServer += UserLeft;

    }

    void OnDisable()
    {
        ParticipantsTest.SendUsersToList -= SendUsersToList;

        ParticipantsReceiver.LeavingServer -= UserLeft;
    }

}
