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

    void OnEnable()
    {
        NetworkManagerGC.UserAdded += AddUser;
        ParticipantsReceiver.RemoveUser += RemoveUser;
    }

    void OnDisable()
    {
        NetworkManagerGC.UserAdded -= AddUser;
        ParticipantsReceiver.RemoveUser -= RemoveUser;        

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        BuildLocalList();
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
        if (!isClient) return;
        UserID objectToAdd = new UserID(user);
        UserIDs.Add(objectToAdd);
        players.Add(user);

        BuildLocalList();
    }


    void RemoveUser(string userToRemove)
    {
        print("You're done! "+userToRemove);
        
        if (isServer) 
        {

            print("You're the server.  Go ahead and remove.");
            foreach (UserID u in UserIDs)
            {
                if (u.userID == userToRemove)
                {
                    print("match! "+userToRemove);

                    UserID objectToRemove = new UserID(userToRemove);
                    UserIDs.Remove(objectToRemove);
                    players.Remove(userToRemove);
                }
            }

            SendUsersToList?.Invoke(players);
        }        
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
