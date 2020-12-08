using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityAtoms.BaseAtoms;
using MutCommon;
using System.Text.RegularExpressions;

// Server only thing
public class ServerPlaylist : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private FloatReference serverTimeToLoadNextVideo;

    [Header("SyncVars")]
    [SerializeField] private int currentIndex;

    [SerializeField] private List<string> queue;

    [Header("References")]
    [SerializeField]
    private YoutubePlayer youtubePlayer;

    [SerializeField]
    private StringReference alias;

    [SerializeField]
    private StringEvent playVideoEvent;

    [SerializeField] private StringEvent playlistEvent;

    [SerializeField] private StringReference addCommandString;

    [SerializeField] private StringReference removeCommandString;

    [SerializeField] private StringReference skipCommandString;


    private float serverTimeVideoOver;

    public override void OnStartServer()
    {
        youtubePlayer.OnVideoFinished.AddListener(() =>
        {
            print("VIDEO FINISHED");
            this.DoAfterTime(serverTimeToLoadNextVideo.Value, () =>
            {
                ServerMoveNext();
            });
        });
    }

    void Start()
    {
        playlistEvent.Register(s =>
        {
            var parameters = Regex.Replace(s, @"\s+", " ").Split(new[] { ' ' }, 3);
            var aliasAttempt = parameters[0];
            var command = parameters[1];
            if (aliasAttempt.ToLowerInvariant() != alias.Value.ToLowerInvariant()) return;
            if (command.ToLowerInvariant() == addCommandString.Value.ToLowerInvariant())
            {
                var videoId = parameters[2];
                CmdEnqueue(videoId);
            }
            else if (command.ToLowerInvariant() == removeCommandString.Value.ToLowerInvariant())
            {
                var videoId = parameters[2];
                CmdRemove(videoId);
            }
            else if (command.ToLowerInvariant() == skipCommandString.Value.ToLowerInvariant())
            {
                CmdRequestNext();
            }
        });
    }


    public string currentId => queue[currentIndex % queue.Count];

    private void ServerMoveNext()
    {
        if (queue.Count == 0) return;
        currentIndex = (currentIndex + 1) % queue.Count;
        playVideoEvent.Raise($"{alias.Value} {currentId}");
    }

    [Command(ignoreAuthority = true)]
    public void CmdRequestNext()
    {
        ServerMoveNext();
    }

    [Command(ignoreAuthority = true)]
    public void CmdEnqueue(string id)
    {
        queue.Add(id);
    }

    [Command(ignoreAuthority = true)]
    public void CmdRemove(string id)
    {
        queue.RemoveAll(i => i == id);
    }
}
