using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.Events;
using Mirror;
using System.Text.RegularExpressions;

public class VideoPlayerCommandListener : NetworkBehaviour
{
    public StringReference alias;
    [Header("The command receives a string of the format: `alias 'id'|'url'`")]
    public StringEvent videoCommandEvent;
    public UnityEvent<string> youtubeCallback;
    public UnityEvent<string> twitchCallback;

    // Start is called before the first frame update
    void Start()
    {
        videoCommandEvent.Register(s =>
        {
            var parameters = Regex.Replace(s, @"\s+", " ").Split(new[] { ' ' }, 2);
            print(parameters);
            print(parameters[0]);
            print(parameters[1]);
            var aliasAttempt = parameters[0];
            if (aliasAttempt.ToLowerInvariant() != alias.Value.ToLowerInvariant()) return;
            CmdChangeVideo(parameters[1]);
        });
    }

    [Command(ignoreAuthority = true)]
    void CmdChangeVideo(string s)
    {
        RpcChangeVideo(s);
    }

    [ClientRpc]
    public void RpcChangeVideo(string videoId)
    {
        youtubeCallback?.Invoke(videoId);
    }
}
