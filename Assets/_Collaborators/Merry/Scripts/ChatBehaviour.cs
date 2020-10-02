using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

//based on code by Dapper Dino https://www.youtube.com/watch?v=p-2QFmCMBt8&ab_channel=DapperDino

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField]
    private GameObject chatUI = null; //ui chat is contained to

    [SerializeField]
    private TMP_Text chatText = null; //chat text

    [SerializeField]
    private TMP_InputField inputField = null; //input field

    private static event Action<string> OnMessage;

    [Header("Avatar Chat")]
    public Transform avatarTransform;
    public TextMeshPro avatarChat;
    public GameObject playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        print("Chat client connected");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerCamera)
        {
            playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        }

        if (playerCamera)
        avatarTransform.rotation = playerCamera.transform.rotation;


    }


    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;

        // print(GetComponent<CharacterCustomizerScript>().characterName);
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }
        OnMessage -= HandleNewMessage;
    }

    public void HandleNewMessage(string message)
    {
        chatText.text += message;
        // Debug.Log(chatText.text);
    }

    [Client]
    public void Send(string message)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(message)) { return; }

        CmdSendMessage(message);
        inputField.text = string.Empty; //clear text input field 
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        //validate messages here (check for bad language/spam)

        RpcShowAvatarMessage(message);

        string userName = GetComponent<SavedAvatarInfoScript>().userName;
        RpcHandleMessage($"[{/*connectionToClient.connectionId*/userName}]: {message}");
    }

    [ClientRpc]
    private void RpcShowAvatarMessage(string message)
    {
        avatarChat.text = message;
        StartCoroutine(ShowTextTimer());
    }


    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }


    IEnumerator ShowTextTimer()
    {
        yield return new WaitForSeconds(3f);
        avatarChat.text = String.Empty;
    }

}
