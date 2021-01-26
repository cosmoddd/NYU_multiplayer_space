using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ParticipantsTest : NetworkBehaviour
{
    public static event Action<List<string>> SendUsersToList;


    // Here we create a class that inherits from the SyncList class
    // using the struct that we just created 
    // ex. SyncList<struct>
    public class UserIDList : SyncList<UserID> {}

    // Now we can use SyncTrait exactly how you would use List<>
    public UserIDList UserIDs = new UserIDList();

    // Create a struct to store the variables you need
    // The struct can be named anything you want

    // as a normal serialized list
    public List<string> players;
    
    public struct UserID
    {
        public string userID;

        // I'm also creating a constructor here so that we can
        // initialize the values when creating a TraitData object
        public UserID(string _userID)
        {
            userID = _userID;
    
        }
    };

    // public override void OnStartClient()
    // {

    // }
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkManagerGC.UserAdded += AddUser;
        ParticipantsReceiver.LeavingServer += RemoveUser;
        BuildLocalList();
    }

    void OnDestory()
    {
        NetworkManagerGC.UserAdded -= AddUser;
        ParticipantsReceiver.LeavingServer -= RemoveUser; 
    }


    void Update()
    {
        if (isClient)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                BuildLocalList();
            }
        }
    }

    void AddUser(string user)
    {
        // if (!isLocalPlayer) return;

        print($"I am the server and I am adding {user} to the list.");

        UserID objectToAdd = new UserID(user);
        UserIDs.Add(objectToAdd);
        players.Add(user);

        BuildLocalList();
    }


    void RemoveUser(string userToRemove)
    {
        print("You're done! "+userToRemove.ToString());

        print("You're the server.  Go ahead and remove.");
        foreach (UserID u in UserIDs)
        {
            print (u.userID);
            if (u.userID == userToRemove)
            {
                print("match! "+userToRemove);

                // UserID objectToRemove = new UserID(userToRemove);
                UserIDs.Remove(u);
                players.Remove(userToRemove);
            }
        }

        SendUsersToList?.Invoke(players);    
    }

    void BuildLocalList()
    {
        players = new List<string>();

        foreach (UserID u in UserIDs)
        {
            players.Add(u.userID);
        }
        SendUsersToList?.Invoke(players);
    }
}
