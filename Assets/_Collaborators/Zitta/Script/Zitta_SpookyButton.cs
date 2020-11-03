using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_SpookyButton : NetworkBehaviour {
    public float MaxTime;
    [SyncVar] public float CurrentTime;
    [Space]
    public Animator Anim;
    public GameObject Clock;
    public bool LastActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
            CurrentTime -= Time.deltaTime;

        if (CurrentTime <= 0 && !LastActive)
            Play();
        else if (CurrentTime > 0 && LastActive)
            Stop();

        ClockRender();
    }

    public void ClockRender()
    {
        float a = 1 - CurrentTime / MaxTime;
        if (a > 1)
            a = 1;
        else if (a < 0)
            a = 0;
        if (a < 1)
            Clock.transform.localEulerAngles = Vector3.Lerp(Clock.transform.localEulerAngles, new Vector3(0, a * 360, 0), 15 * Time.deltaTime);
        else
            Clock.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void Interact()
    {
        CmdResetTime();
        OnPush();
    }

    [Command(ignoreAuthority = true)]
    public void CmdResetTime()
    {
        CurrentTime = MaxTime;
    }

    public void OnPush()
    {
        Anim.SetTrigger("Push");
    }

    public void Play()
    {
        LastActive = true;
        GetSound().Play();
        Anim.SetBool("Active", true);
    }

    public void Stop()
    {
        LastActive = false;
        GetSound().Stop();
        Anim.SetBool("Active", false);
    }

    public Zitta_SpookySound GetSound()
    {
        return Zitta_SpookySound.Main;
    }
}
