using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using RenderHeads.Media.AVProVideo;

public class Twitch_AvPro : MonoBehaviour {

    /*PRIVATE INFO DO NOT CHANGE THESE URLS OR VALUES*/
    private const string serverURI = "https://unity-dev-youtube.herokuapp.com/api/info?url=https://www.twitch.tv/";
    private const string formatURI = "&format=best&flatten=true";
    /*END OF PRIVATE INFO*/
    [Header("AVPro Media Player")]
    public MediaPlayer mplayer;

    [SerializeField]
    public string liveResult;

    public string twitchChannelID = "RtU_mdL2vBM";
    private string videoUrl;
    //start playing the video
    public bool playOnStart = false;

    public void Start()
    {
        print("attempting to play...");
        if (playOnStart)
        {
            PlayYoutubeVideo(twitchChannelID);
        }
    }

    public void PlayYoutubeVideo(string _videoId)
    {
        twitchChannelID = _videoId;
        StartCoroutine(LiveRequest(twitchChannelID));
    }

    IEnumerator LiveRequest(string videoID)
    {
        // yield return new WaitForSeconds(delayRequest);
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        yield return request;

        if (request == null)
        {
            print("there's nothing here...");
            yield break;
        }
                            // JsonUtility.FromJson
        var requestData = JSON.Parse(request.text);
        liveResult = requestData["videos"][0]["url"];
        StartCoroutine(LivePlay());
    }


    IEnumerator LivePlay() //The prepare not respond so, i forced to play after some seconds
    {
        yield return new WaitForSeconds(0.5f);
        string uri = "";
        uri = liveResult;
        mplayer.m_VideoLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
        mplayer.m_VideoPath = uri;
        StartCoroutine(Play());
    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(4);
        mplayer.OpenVideoFromFile(mplayer.m_VideoLocation, mplayer.m_VideoPath, mplayer.m_AutoStart);
    }

}
