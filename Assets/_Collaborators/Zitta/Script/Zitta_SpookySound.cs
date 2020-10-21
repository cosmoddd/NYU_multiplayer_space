using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_SpookySound : MonoBehaviour {
    public static Zitta_SpookySound Main;
    public AudioSource Source;
    public float VolumeSpeed;
    public float MaxVolume;
    public bool Active;

    public void Awake()
    {
        Main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Active && Source.volume < 1 * MaxVolume)
            Source.volume += VolumeSpeed * Time.deltaTime;
        else if (!Active && Source.volume > 0)
            Source.volume -= VolumeSpeed * Time.deltaTime;
        if (Source.volume > 1 * MaxVolume)
            Source.volume = 1 * MaxVolume;
        else if (Source.volume < 0)
            Source.volume = 0;
    }

    public void Play()
    {
        Active = true;
    }

    public void Stop()
    {
        Active = false;
    }
}
