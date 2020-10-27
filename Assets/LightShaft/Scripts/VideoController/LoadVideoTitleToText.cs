using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LoadVideoTitleToText : MonoBehaviour {

    public TextMesh textMesh;
    public YoutubePlayer player;

    public void SetVideoTitle()
    {
        textMesh.text = player.GetVideoTitle();
    }

}
