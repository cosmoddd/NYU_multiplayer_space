using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_ServerPlatform : NetworkBehaviour {
    public bool Active;
    public float Delay;
    public float ActiveDuration;
    public float EmptyDuration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            Delay -= Time.deltaTime;
            if (Delay <= 0)
            {
                if (Active)
                {
                    Active = false;
                    Delay = EmptyDuration;
                }
                else
                {
                    Active = true;
                    Delay = ActiveDuration;
                }
            }

            if (Active)
                transform.localScale = new Vector3(1, 1, 0);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
