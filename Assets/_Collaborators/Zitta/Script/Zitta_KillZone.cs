using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_KillZone : MonoBehaviour {
    public static Zitta_KillZone Main;
    public Zitta_CheckPoint CurrentCP;

    private void Awake()
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
        
    }

    public void OnTriggerEnter(Collider C)
    {
        MoveController MC = null;
        if (C.transform.GetComponent<MoveController>())
        {
            MC = C.transform.GetComponent<MoveController>();
        }
        if (!MC)
            return;
        if (!MC.isLocalPlayer)
            return;
        Respawn(MC.gameObject);
    }

    public void Respawn(GameObject G)
    {
        if (!CurrentCP)
            return;
        StartCoroutine(RespawnIE(G));
    }

    public IEnumerator RespawnIE(GameObject G)
    {
        float a = 0f;
        while (a <= 0.25f)
        {
            G.transform.position = CurrentCP.GetSpawnPosition();
            a += Time.deltaTime;
            yield return 0;
        }
    }
}
