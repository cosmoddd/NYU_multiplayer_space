using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncTube : NetworkBehaviour
{
    [Header("config")]
    public bool useRawURL = false;

    [Header("SyncVars")]
    //[SyncVar(hook = nameof(NewServerUrl))]
    [SyncVar]
    [SerializeField] private string serverURL;

    [SyncVar]
    [SerializeField] private string serverYoutubeURL;

    [SyncVar]
    [SerializeField] private int serverPlaybackSeconds;

    [Header("references")]

    [SerializeField] private YoutubePlayer youtubePlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        SyncWithServer();
    }

    void Update()
    {
        // Update is called once per frame
        if (isServer)
        {
            serverPlaybackSeconds = (int)youtubePlayer.currentVideoDuration;
            serverURL = youtubePlayer.videoUrl;
            serverYoutubeURL = youtubePlayer.youtubeUrl;
        }
        else
        {
            //CheckVideoOverAndUpdate(serverURL);
        }
    }

    [Command]
    void CmdSyncAll()
    {
        RpcSyncAll();
    }

    [ClientRpc]
    void RpcSyncAll()
    {
        SyncWithServer();
    }

    void CheckVideoOverAndUpdate(string updatedUrl)
    {
        // if video is over
        if (youtubePlayer.percentageOver >= 1 || youtubePlayer.percentageOver == 0 || !youtubePlayer.videoPlayer.isPlaying)
        {
            // if serverURL is different than the current video playing
            if (youtubePlayer.videoUrl != updatedUrl)
            {
                youtubePlayer.Stop();
                PlayServerVideo();
            }
        }
    }

    private void PlayServerVideo()
    {
        if (useRawURL)
        {
            youtubePlayer.PlayFromLoadedUrl(serverURL);
        }
        else
        {
            youtubePlayer.Play(serverYoutubeURL);
        }
    }

    //void NewServerUrl(string oldValue, string newValue)
    //{
    //CheckVideoOverAndUpdate(newValue);
    //}

    void SyncWithServer()
    {
        if (string.IsNullOrWhiteSpace(serverURL)) return;
        youtubePlayer.Stop();
        youtubePlayer.startFromSecond = true;
        youtubePlayer.startFromSecondTime = serverPlaybackSeconds;
        //youtubePlayer.youtubeUrl = serverURL;
        PlayServerVideo();
    }
}
