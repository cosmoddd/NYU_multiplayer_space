using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ParticipantsTest : NetworkBehaviour
{
    public Action<List<string>> SendUsersToList;

    public List<string> players;
    // Here we create a class that inherits from the SyncList class
    // using the struct that we just created 
    // ex. SyncList<struct>
    public class UserIDList : SyncList<UserID> {}

    // Now we can use SyncTrait exactly how you would use List<>
    public UserIDList UserIDs = new UserIDList();

    // Create a struct to store the variables you need
    // The struct can be named anything you want
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

    void OnEnable()
    {
        NetworkManagerGC.UserAdded += AddUser;
    }

    void OnDisable()
    {
        NetworkManagerGC.UserAdded -= AddUser;
    }


    void AddUser(string user)
    {
        // When adding objects to the SyncList
        // ensure that you are creating objects from the struct you used
        // to create the custom list class
        UserID exampleObject = new UserID(user);
        UserIDs.Add(exampleObject);
        players.Add(user);
        
        List<string> userList = new List<string>();

        foreach (UserID u in UserIDs)
        {
            userList.Add(u.userID);
        }
        
        SendUsersToList?.Invoke(userList)
    }
}
