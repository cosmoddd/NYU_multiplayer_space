using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class UI_ParticipantsList : NetworkBehaviour //: MonoBehaviour
{

    public struct UserID
    {
        //what all makes up a struct
        public string name;

        //used when making a new struct
        public UserID(string uName)
        {
            name = uName;
        }
    };

    public class UserIDList : SyncList<UserID> { }
    public UserIDList userList = new UserIDList();

    //note to self: 
    //1. be sure to have using System included when using events
    //2. declare event at top like so
    //3. call event when we want it
    //4. subscribe to event where we need it
    //5. ping to a new function with what we wanted that event to trigger
    public static event Action<List<string>> UsersUpdated;

    public List<string> players = new List<string>();

    void AddUser(string user)
    {
        if (!isClient)
        {
            return;
        }

        UserID nameToAdd = new UserID(user);
        userList.Add(nameToAdd);
        players.Add(user);

        RebuildList();
    }

    void RebuildList()
    {
        players = new List<string>();
        foreach(UserID x in userList)
        {
            players.Add(x.name);
        }

        Debug.Log("Sending list");
        UsersUpdated?.Invoke(players); //use this to call the event
    }

    void RemoveUser(string removedUser)
    {
        if (isServer)
        {
            foreach(UserID x in userList)
            {
                if(x.name == removedUser)
                {
                    userList.Remove(x);
                    players.Remove(removedUser);
                }
            }

            UsersUpdated?.Invoke(players);
        }
    }

    private void Update()
    {
        if (isClient && Input.GetKeyDown(KeyCode.Tab)) {}
        {
            RebuildList();
        }
    }

    //use these so that its registered for the events we wanna use here
    private void OnEnable()
    {
        NetworkManagerGC.UserAdded += AddUser;
        UI_ClientParticipantsList.ClientLeft += RemoveUser;
    }

    private void OnDisable()
    {
        NetworkManagerGC.UserAdded -= AddUser;
        UI_ClientParticipantsList.ClientLeft -= RemoveUser;
    }


}