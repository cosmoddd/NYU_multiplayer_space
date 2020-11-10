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
    [Header("The command receives a string of the format: `alias y(outube)?|t(witch) 'id'|'url'`")]
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
    void RpcChangeVideo(string s)
    {
        var parameters = s.Split();
        var callback = parameters[0].StartsWith("y") ? youtubeCallback : twitchCallback;
        print(s);
        var videoId = parameters[1];
        callback?.Invoke(videoId);
    }
}
