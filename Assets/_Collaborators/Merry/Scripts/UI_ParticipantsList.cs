using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class UI_ParticipantsList //: MonoBehaviour
{

    //player list
    public static List<GameObject> playerList = new List<GameObject>(); //player list individual player instances can access
    private static bool isMod = false;
    private static string playerListString;

    private static int playerCount = 0;



    void Update()
    {

    }

    public static void AddPlayer(GameObject player, bool m) //called from player object
    {
        playerList.Add(player);
        isMod = m;

        playerCount++;

        UpdatePlayerList();
    }

    public static void RemovePlayer(GameObject player, bool m) //called from player object
    {
        //find player in list
        //delete in list
        //find in string
        
        playerCount--;

        UpdatePlayerList();
    }

    public static void UpdatePlayerList()
    {
        string tempString = "Online: ";

        for (int i = 0; i < playerCount; i++)
        {
            tempString += "\n [ " + i + " ] " + playerList[0].name;
        }

        playerListString = tempString;
    }

    //send it client side
}
