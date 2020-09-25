using Mirror;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Video;

public class MovieTheatre : MonoBehaviour //NetworkBehaviour
{
  public UnityEngine.Video.VideoClip videoClip;
  private VideoPlayer videoPlayer;

  public FloatVariable videoElapsed01;

  void Start()
  {
    videoPlayer = gameObject.AddComponent<VideoPlayer>();
    var audioSource = gameObject.AddComponent<AudioSource>();

    videoPlayer.playOnAwake = false;
    videoPlayer.clip = videoClip;
    videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
    videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
    videoPlayer.targetMaterialProperty = "_MainTex";
    videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
    videoPlayer.SetTargetAudioSource(0, audioSource);
    print(videoClip.width);
    print(videoClip.height);
    transform.localScale = Vector3.Scale(transform.localScale, new Vector3((float)videoClip.width / (float)videoClip.height, 1, 1));
  }

  void Update()
  {
    videoElapsed01.Value = (float)(videoPlayer.time / videoPlayer.length);
  }

  public void SetVideoPosition(float position)
  {
    videoPlayer.time = position * videoPlayer.length;
  }

  public void Pause() => videoPlayer.Pause();
  public void Stop() => videoPlayer.Stop();
  public void Play() => videoPlayer.Play();
}
