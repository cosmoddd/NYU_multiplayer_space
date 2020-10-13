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
    [SyncVar]
    [SerializeField] private string currentTrackName;

    [SyncVar]
    [SerializeField] private float serverPlaytime;

    [SerializeField] private float clientPlaytime;

    [SerializeField] private string TracksPath = "Jukebox";
    [SerializeField] private List<AudioClip> tracks;

    [SerializeField] private StringValueList trackNameList;
    [SerializeField] private StringVariable currentTrackNameVariable;
    [SerializeField] private FloatVariable currentTrackElapsed;

    [SerializeField] private readonly SyncListString queue = new SyncListString();

    private AudioSource audioSource;

    private AudioClip currentTrack => tracks.FirstOrDefault(t => t.name == currentTrackName);

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
        currentTrackNameVariable.Value = currentTrackName;
        clientPlaytime = audioSource.time;

        if (isServer) serverPlaytime = clientPlaytime;

        if (audioSource.clip != null)
        {
            currentTrackElapsed.Value = clientPlaytime / audioSource.clip.length;
        }
        else
        {
            currentTrackElapsed.Value = 0;
        }

        if (audioSource.clip == null || clientPlaytime >= audioSource.clip.length)
        {
            // Song is over call for next in queue;
            if (isServer)
            {
                ServerDequeueSong();
            }

            if (audioSource.clip?.name != currentTrackName)
            {
                audioSource.clip = currentTrack;
                audioSource.Play();
                clientPlaytime = 0;
            }
        }
    }

    #endregion

    public void SyncWithServer()
    {
        audioSource.clip = currentTrack;
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }

        audioSource.time = serverPlaytime;
    }

    // Runs server queue
    private void ServerDequeueSong()
    {
        // Remove current element of top of queue on the server
        if (queue == null || queue.Count() == 0)
        {
            Debug.Log("No more songs in queue");
            audioSource.clip = null;
            return;
        }

        // Server should always dequeue first, so no real reason to worry for some race condition going on here
        // Might be nice to call Sync every once in a while to be sure it's synced (can be called on a region outside of the listening radius)

        currentTrackName = queue.First();
        queue.RemoveAt(0);
    }

    private void Skip()
    {
        if (isClient)
        {
            CmdSkipSong();
        }
        else
        {
            RpcSkipSong();
        }
    }

    [Command]
    private void CmdSkipSong() => RpcSkipSong();

    [ClientRpc]
    private void RpcSkipSong() => ServerDequeueSong();

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

