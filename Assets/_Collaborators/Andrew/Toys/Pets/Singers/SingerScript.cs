using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingerScript : MonoBehaviour
{
    AudioSource AS;
    public AudioClip cry;

    public Transform jaw;
    public Transform eyes;
    public Transform head;

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ToyIdentityScript>().active)
        {
            AS.PlayOneShot(cry,.1f);
            //jaw.localRotation = Quaternion.Euler(42, 0, 0);
            GetComponent<ToyIdentityScript>().active = false;
        }
        if (AS.isPlaying)
        {
            jaw.localRotation = Quaternion.Lerp(jaw.localRotation, Quaternion.Euler(42, 0, 0), Time.deltaTime * 15);
            eyes.localScale = Vector3.Lerp(eyes.localScale, new Vector3(0.1840736f, 0.1840736f, 0.06571979f), Time.deltaTime * 6);
            head.localScale = Vector3.Lerp(head.localScale,Vector3.one*1.05f,Time.deltaTime * 10);
        }
        else
        {
            jaw.localRotation = Quaternion.Lerp(jaw.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 6);
            eyes.localScale = Vector3.Lerp(eyes.localScale,Vector3.one * 0.1840736f,Time.deltaTime * 6);
            head.localScale = Vector3.Lerp(head.localScale, Vector3.one, Time.deltaTime * 10);
        }

    }
}
