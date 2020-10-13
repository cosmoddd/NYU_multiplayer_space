using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using System.Linq;
using UnityEngine.Serialization;

//based on code by Dapper Dino https://www.youtube.com/watch?v=p-2QFmCMBt8&ab_channel=DapperDino

public class ChatBehaviour : NetworkBehaviour
{
  [SerializeField]
  private GameObject chatUI = null; //ui chat is contained to

  [SerializeField]
  private TMP_Text chatText = null; //chat text

  [SerializeField]
  private TMP_InputField inputField = null; //input field
  public Button sendButton;

  [SerializeField]
  private Image chatBackground = null; //chat background + goes with slider

  private static event Action<string> OnMessage;

  public GameObject playerCamera;
  [Header("Chat UI")]
  public Transform avatarTransform;
  public TextMeshPro avatarChat;

  [Header("Chat Mode Control")]
  public BoolVariable inChatMode;

  [Header("Chat commands")]
  public StringEvent[] SpecificChatCommands;
  public StringEvent GenericChatCommand;
  public bool SendCommandsToChat;
  public char CommandPrefix = '/';

  public override void OnStartClient()
  {
    base.OnStartClient();

    if (inChatMode.Value == false)
    {
      // print("DISABLE YOU!");

      inputField.gameObject.SetActive(false);
      chatBackground.gameObject.SetActive(false);
      // sendButton.gameObject.SetActive(false);
    }
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

  // tab button determines whether or not chat mode is enbaled
  void Update()
  {
    if (isLocalPlayer && Input.GetKeyDown(KeyCode.Tab))
    {
      inChatMode.Value = !inChatMode.Value;

      if (inChatMode.Value == false)
      {
        inputField.gameObject.SetActive(false);
        chatBackground.gameObject.SetActive(false);
        // sendButton.gameObject.SetActive(false);
      }
      if (inChatMode.Value == true)
      {
        inputField.gameObject.SetActive(true);
        chatBackground.gameObject.SetActive(true);
        // sendButton.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        // inputField.MoveTextStart(true);
      }
    }

    if (isLocalPlayer && Input.GetKeyDown(KeyCode.Escape))
    {
      if (inChatMode.Value == true)
      {
        DisableChatMode();
        return;
      }
    }

    // return enables chat box if it's disabled
    if (isLocalPlayer && Input.GetKeyDown(KeyCode.Return))
    {
      StartCoroutine(EnterChatToggle());
    }

  }


  // need to do this in a coroutine to avoid simultaneous frame conflict with enabling/disabling
  IEnumerator EnterChatToggle()
  {
    // yield return null;

    // enable chat mode if disabled
    if (!inputField.gameObject.activeInHierarchy && inChatMode.Value == false)
    {
      print("ENABLE You!");

      inChatMode.Value = true;
      inputField.gameObject.SetActive(true);
      chatBackground.gameObject.SetActive(true);
      inputField.Select();
      inputField.ActivateInputField();
      yield break;
    }

    if (inChatMode.Value == true && string.IsNullOrWhiteSpace(inputField.text))
    {
      print("I'm outta here");
      DisableChatMode();
      yield break;
    }

    if (inChatMode.Value == true && inputField.gameObject.activeInHierarchy)
    {
      print("Send the text");
      Send(inputField.text);
      yield break;
    }
  }

  void DisableChatMode()
  {
    inputField.gameObject.SetActive(false);
    chatBackground.gameObject.SetActive(false);
    inChatMode.Value = false;
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


  public bool ParseCommandAndInvoke(string message)
  {
    var cleanMessage = message.Trim();
    if (cleanMessage[0] != '/')
    {
      return false;
    }

    var splitCommand = cleanMessage.Substring(1).Split(new char[] { ' ' }, 2);
    var commandEvent = SpecificChatCommands.FirstOrDefault(c => c.name == splitCommand[0]);
    if (commandEvent == null)
    {
      GenericChatCommand.Raise(splitCommand[0]);
    }
    else
    {
      commandEvent.Raise(splitCommand.Length > 1 ? splitCommand[1] : "");
    }

    return true;
  }

  [Client]
  public void Send(string message)
  {
    if (!Input.GetKeyDown(KeyCode.Return)) { return; }
    if (string.IsNullOrWhiteSpace(message)) { return; }

    // Check if the message is a command
    if (ParseCommandAndInvoke(message) && !SendCommandsToChat) return;

    CmdSendMessage(message);
    inputField.text = string.Empty; //clear text input field 
    inputField.Select();
    inputField.ActivateInputField();
    // inputField.MoveTextStart(true);
  }

  [Client]
  public void SendButton()
  {
    if (string.IsNullOrWhiteSpace(inputField.text)) { return; }

    CmdSendMessage(inputField.text);
    inputField.text = string.Empty; //clear text input field  
  }

  [Command]
  private void CmdSendMessage(string message)
  {
    //validate messages here (check for bad language/spam)

    RpcShowAvatarMessage(message);

    string userName = GetComponent<SavedAvatarInfoScript>().userName;
    RpcHandleMessage($"<color=white>[{userName}]</color>: {message}");
  }

  [ClientRpc]
  private void RpcShowAvatarMessage(string message)
  {
    avatarChat.text = message;
    StopAllCoroutines();
    StartCoroutine(ShowTextTimer());
  }


  [ClientRpc]
  private void RpcHandleMessage(string message)
  {
    OnMessage?.Invoke($"\n{message}");
  }


  IEnumerator ShowTextTimer()
  {
    yield return new WaitForSeconds(5f);
    avatarChat.text = String.Empty;
  }

}
