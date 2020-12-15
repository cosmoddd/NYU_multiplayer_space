using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class OnlineJukebox : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnServerTrackChanged))]
    [SerializeField] private string currentTrackInServerName;

    [SyncVar(hook = nameof(OnServerTimeChanged))]
    [SerializeField] private float serverPlaytime;

    [SerializeField] private float clientPlaytime;

    [SerializeField] private string TracksPath = "Jukebox";
    [SerializeField] private List<AudioClip> tracks;

    [SerializeField] private StringValueList trackNameList;
    [SerializeField] private StringVariable currentTrackNameVariable;
    [SerializeField] private FloatVariable currentTrackElapsed;

    private bool ShouldRandomizeQueueOnEmpty = true;

    [SerializeField] private readonly SyncListString queue = new SyncListString();

    private AudioSource audioSource;

    private AudioClip GetTrackByName(string name) => tracks.FirstOrDefault(t => t.name == name);
    private AudioClip currentTrack => GetTrackByName(currentTrackInServerName);

    [SyncVar]
    public bool paused = false;

    #region Public Methods 
    #endregion

    #region Network Behaviour

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        tracks = Resources.LoadAll<AudioClip>(TracksPath).ToList();
        tracks.OrderBy(c => c.name);

        // Populate trackList atom
        trackNameList.List = tracks.Select(t => t.name).ToList();
    }

    public override void OnStartClient()
    {
        SyncWithServer();
    }

    private void Update()
    {
        // if (paused) return;

        currentTrackNameVariable.Value = currentTrackInServerName;

        if (audioSource.isPlaying)
        {
            clientPlaytime = audioSource.time;
            if (isServer) serverPlaytime = clientPlaytime;
        }

        // Update local current track elapsed time
        if (audioSource.clip != null)
        {
            currentTrackElapsed.Value = clientPlaytime / audioSource.clip.length;
        }
        else
        {
            currentTrackElapsed.Value = 0;
        }

        // If it should be playing but it's not
        if ((!paused && !audioSource.isPlaying))
        {
            // print("Song is over");
            // Song is over call for next in queue;
            if (isServer)
            {
                ServerDequeueSong();
            }

            // if the local clip doesn't match the server's start playing the server's one
            if (audioSource.clip?.name != currentTrackInServerName)
            {
                print("Playing next thing" + currentTrack);
                audioSource.Stop();
                audioSource.clip = currentTrack;
                audioSource.time = serverPlaytime;
                audioSource.Play();
                clientPlaytime = 0;
            }
        }
    }

    #endregion

    public void SyncWithServer()
    {
        audioSource.clip = currentTrack;
        if (audioSource.clip != null && !paused)
        {
            audioSource.Play();
        }
        isWaitingForServerTimeChange = true;
        audioSource.time = serverPlaytime;
    }

    private bool isWaitingForServerTrackChange;
    private void OnServerTrackChanged(string oldTrackName, string newTrackName)
    {
        if (isWaitingForServerTrackChange)
        {
            SyncWithServer();
            isWaitingForServerTrackChange = false;
        }
    }
    private bool isWaitingForServerTimeChange;
    private void OnServerTimeChanged(float oldTime, float newTime)
    {
        if (isWaitingForServerTimeChange)
        {
            audioSource.time = newTime;
            isWaitingForServerTimeChange = false;
        }

    }



    // Runs server queue
    private void ServerDequeueSong()
    {
        // Remove current element of top of queue on the server
        if (queue.Count() == 0)
        {
            if (ShouldRandomizeQueueOnEmpty)
            {
                trackNameList.OrderBy(_ => Random.value - 0.5)
                  .ToList()
                  .ForEach(queue.Add);

                // So it doesn't immediatly repeat the current track, add it to some random position after the next song
                queue.Remove(currentTrackInServerName);
                queue.Insert(Random.Range(1, queue.Count()), currentTrackInServerName);
            }
            else
            {
                // Debug.Log("No more songs in queue");
                audioSource.clip = null;
                return;
            }
        }

        // Server should always dequeue first, so no real reason to worry for some race condition going on here
        // Might be nice to call Sync every once in a while to be sure it's synced (can be called on a region outside of the listening radius)

        currentTrackInServerName = queue.First();
        queue.RemoveAt(0);

        audioSource.Stop();
        audioSource.clip = currentTrack;
        audioSource.Play();
        audioSource.time = 0;
        serverPlaytime = 0;
        clientPlaytime = 0;
    }

    public void Skip()
    {
        if (isClient)
        {
            CmdSkipSong();
        }
        else
        {
            ServerDequeueSong();
            RpcSkipSong();
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdPlayPause()
    {
        RpcPlayPause();
    }

    [ClientRpc]
    void RpcPlayPause()
    {
        if (!paused)
        {
            print("pausing!");
            audioSource.Pause();
            paused = true;
            return;
        }
        else
        {
            print("unpausing!");
            audioSource.UnPause();
            paused = false;
            SyncWithServer();
            return;
        }
    }


    [Command(ignoreAuthority = true)]
    private void CmdSkipSong()
    {
        ServerDequeueSong();
        RpcSkipSong();
    }

    [ClientRpc]
    void RpcSkipSong()
    {
        SyncWithServer();
        isWaitingForServerTrackChange = true;
    }

    public void EnqueueRandomSong() => EnqueueRandomSongFromList(trackNameList.List);
    public void EnqueueRandomUniqueSong() => EnqueueRandomSongFromList(trackNameList.Except(queue).ToList());
    public void EnqueueSong(string songName) => CmdEnqueueSong(songName);

    private void EnqueueRandomSongFromList(List<string> options) => EnqueueSong(options.ElementAt(Random.Range(0, options.Count())));

    [Command(ignoreAuthority = true)]
    void CmdEnqueueSong(string songName)
    {
        Debug.Log($"enqueueing new song: {songName}");
        if (!trackNameList.Contains(songName))
        {
            Debug.LogError($"No track named {songName}");
            return;
        }
        queue.Add(songName);
    }
}

