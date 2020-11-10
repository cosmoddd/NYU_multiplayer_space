using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
 
public class Zitta_QuantumObject : NetworkBehaviour {
    public List<Zitta_QuantumSpot> Spots;
    public Zitta_QuantumSpot CurrentSpot;
    public bool Discovered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        if (!Discovered && CurrentSpot.VisibleTime > 0)
            Discovered = true;

        if (Discovered && CurrentSpot.VisibleTime <= 0)
            ChangeSpotProcess();
    }

    public void ChangeSpotProcess()
    {
        List<Zitta_QuantumSpot> Sps = new List<Zitta_QuantumSpot>();
        foreach (Zitta_QuantumSpot Spot in Spots)
        {
            if (Spot.VisibleTime <= 0 && Spot != CurrentSpot)
                Sps.Add(Spot);
        }
        if (Sps.Count > 0)
            ChangeSpot(Sps[Random.Range(0, Sps.Count)]);
    }

    public void ChangeSpot(Zitta_QuantumSpot Spot)
    {
        transform.position = Spot.transform.position;
        CurrentSpot = Spot;
        Discovered = false;
    }
}
