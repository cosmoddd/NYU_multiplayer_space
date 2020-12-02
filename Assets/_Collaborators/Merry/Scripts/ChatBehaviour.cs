using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using System.Linq;

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
    public static event Action LoggedIn;

    public static event Func<string> RetrievePlayerList;



    public GameObject playerCamera;
    [Header("Chat UI")]
    public Transform avatarTransform;
    public TextMeshPro avatarChat;
    public AudioSource avatarChatAudio;
    public TextMeshPro avatarName;

    [Header("Chat Mode Control")]
    public BoolVariable inChatMode;

    [Header("Chat Commands")]
    [SerializeField] private string SpecificChatCommandsPath = "Atoms/ChatEvents";
    public StringEvent[] SpecificChatCommands;
    public StringEvent GenericChatCommand;
    public bool SendCommandsToChat;
    public char CommandPrefix = '/';


    [Header("Participants Control")]
    [SerializeField]
    public Canvas participantsListCanvas = null; //participants list
    public BoolVariable participantsListActive;
    [SerializeField]
    private TMP_Text participantsText = null;
    private String participantID;

    private bool hasMod = false; //intergrated with authenticator and tags player as a mod

    [Header("Emote Control")]
    [SerializeField]
    public GameObject emoteList = null; //participants list
    public BoolVariable emoteListActive;

    public override void OnStartClient()
    {
        base.OnStartClient();

        //check if player is a mod and grant it

        //add player name & mod status to participants list in UI_ParticipantsList.cs

        //retrieve the participants list
        participantsText.text = RetrievePlayerList?.Invoke();

        SpecificChatCommands = Resources.LoadAll<StringEvent>(SpecificChatCommandsPath);

        if (inChatMode.Value == false)
            {
                // print("DISABLE YOU!");

                inputField.gameObject.SetActive(false);
            chatBackground.gameObject.SetActive(false);
            if (emoteList) emoteList.SetActive(false);
            // sendButton.gameObject.SetActive(false);
        }

        avatarName.text = GetComponent<MeshAssigner>().userName;
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
    // esc brings up the participants list
    void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Tab))
        {
            inChatMode.Value = !inChatMode.Value;

            if (inChatMode.Value == false)
            {
                inputField.gameObject.SetActive(false);
                chatBackground.gameObject.SetActive(false);
                participantsListCanvas.enabled = false;
                if (emoteList) emoteList.SetActive(false);
                // sendButton.gameObject.SetActive(false);
            }
            if (inChatMode.Value == true)
            {
                inputField.gameObject.SetActive(true);
                chatBackground.gameObject.SetActive(true);
                participantsListCanvas.enabled = true;
                if (emoteList) emoteList.SetActive(false);
                // sendButton.gameObject.SetActive(true);
                inputField.Select();
                inputField.ActivateInputField();
                // inputField.MoveTextStart(true);
            }

        }

        //EMOTE STUFF
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Slash))
        {
            inChatMode.Value = !inChatMode.Value;

            if (inChatMode.Value == false)
            {
                inputField.gameObject.SetActive(false);
                chatBackground.gameObject.SetActive(false);
                if (emoteList) emoteList.SetActive(false);
                participantsListCanvas.enabled = false;
                // sendButton.gameObject.SetActive(false);
            }
            if (inChatMode.Value == true)
            {
                inputField.gameObject.SetActive(true);
                chatBackground.gameObject.SetActive(true);
                if (emoteList) emoteList.SetActive(true); //panel with all emotes
                participantsListCanvas.enabled = false;

                inputField.Select();
                inputField.text = "/";
                inputField.ActivateInputField();
                // inputField.MoveTextStart(true);

                // Start a coroutine to deselect text and move caret to end. 
                // This can't be done now, must be done in the next frame.
                StartCoroutine(MoveTextEnd_NextFrame());
            }
        }
        // return enables chat box if it's disabled
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(EnterChatToggle());
        }

    }

    IEnumerator MoveTextEnd_NextFrame()
    {
        yield return 0; // Skip the first frame in which this is called.
        inputField.MoveTextEnd(false); // Do this during the next frame.
    }

    // need to do this in a coroutine to avoid simultaneous frame conflict with enabling/disabling
    IEnumerator EnterChatToggle()
    {
        if (inChatMode.Value == true && string.IsNullOrWhiteSpace(inputField.text))
        {
          yield return null;
          print("I'm outta here");
          DisableChatMode();
          yield break;
        }

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
        if (emoteList) emoteList.SetActive(false);
        participantsListCanvas.enabled = false;
    }

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
        
        LoggedIn?.Invoke();

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

        // Check if the message is a command
        if (!(ParseCommandAndInvoke(message) && !SendCommandsToChat))
        {
            CmdSendMessage(message);  // if the message is not consumed, send it to chat
        }

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

        string userName = GetComponent<MeshAssigner>().userName;
        RpcHandleMessage($"<color=white>[{userName}]</color>: {message}");
    }

    [ClientRpc]
    private void RpcShowAvatarMessage(string message)
    {
        avatarChat.text = message;
        float randomPitch = UnityEngine.Random.Range(.8f,1.2f);
        avatarChatAudio.pitch = randomPitch;
        avatarChatAudio.Play();
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


  public bool ParseCommandAndInvoke(string message)
  {

    var cleanMessage = message.Trim();
    if (cleanMessage[0] != CommandPrefix)
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

}
