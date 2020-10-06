using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_CheckPoint : MonoBehaviour {
    public bool StartCheckpoint;
    public Vector3 SpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (StartCheckpoint)
            Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider C)
    {
        MoveController MC = null;
        if (C.transform.GetComponent<MoveController>())
            MC = C.transform.GetComponent<MoveController>();
        if (!MC)
            return;
        if (!MC.isLocalPlayer)
            return;
        Activate();
    }

    public void Activate()
    {
        Zitta_KillZone.Main.CurrentCP = this;
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position + SpawnPosition;
    }
}
