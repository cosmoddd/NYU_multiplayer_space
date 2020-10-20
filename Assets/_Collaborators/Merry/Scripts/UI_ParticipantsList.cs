using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class UI_ParticipantsList : NetworkBehaviour //: MonoBehaviour
{

    //player list
    // public SyncList<NameAndModStatus> playerListMod;// = new List<String>(); //player list individual player instances can access


    private SyncListString playerList = new SyncListString();


    [SyncVar]
    public string playerListString;

    [SyncVar]
    int playerCount = 0;
  
    public static event Action<string> RecieveList;

    [TextArea(2,10)]
    public string listTextArea;

    public void OnEnable()
    {
        CharacterCustomizerScript.NameReady += AddPlayer;   
        CharacterCustomizerScript.NameUnready += RemovePlayer;
        ChatBehaviour.RetrievePlayerList += ReturnList;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        CharacterCustomizerScript.NameReady -= AddPlayer;
        CharacterCustomizerScript.NameUnready -= RemovePlayer;
        ChatBehaviour.RetrievePlayerList -= ReturnList;
    }

    public void AddPlayer(String player, bool m) //called from player object
    {
        // if (!playerList.Contains(player))
        // {
        //     print($"{player} added.");
        //     playerList.Add(player);
        //     playerCount++;
        //     CmdUpdatePlayerList();
        // }
    }

    public void RemovePlayer(String player, bool m) //called from player object
    {
        // if (playerList.Contains(player))
        // {
        //     playerList.Remove(player);
        // }

        print($"{player} is outta here.");
        playerCount--;
        CmdUpdatePlayerList();
    }

    [Command(ignoreAuthority = true)]
    public void CmdUpdatePlayerList()
    {
        listTextArea = "Online: ";
        for (int i = 0; i < playerCount; i++)
        {
            listTextArea += "\n [ " + i + " ] " + playerList[i];
        }
        RpcSetList(listTextArea);
    }

    [ClientRpc]
    void RpcSetList(string _list)
    {
        RecieveList?.Invoke(_list);
    }


    string ReturnList()
    {
        return listTextArea;
    }

}

public struct SuperString
{
    public string name;
}

[System.Serializable] 
public class NameAndModerator
{
    public string name;
    public bool isModerator;
}
