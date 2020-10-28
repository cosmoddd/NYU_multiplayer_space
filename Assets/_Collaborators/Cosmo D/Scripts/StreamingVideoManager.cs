using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamingVideoManager : MonoBehaviour
{

    public float breakBetweenStreams;
    public GameObject[] streamObjects;

    void Start()
    {
        StartCoroutine(QueueNextStream());
    }

    IEnumerator QueueNextStream()
    {
        foreach (GameObject streamObject in streamObjects)
        {
            yield return new WaitForSeconds(breakBetweenStreams);
            print("Launching next stream.");
            streamObject.SetActive(true);
        }
    }

}
