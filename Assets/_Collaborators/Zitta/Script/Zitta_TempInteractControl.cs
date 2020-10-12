using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_TempInteractControl : MonoBehaviour {
    public static Zitta_TempInteractControl Main;
    public List<Zitta_TempInteractScript> Interacts;

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
        if (Input.GetKeyDown(KeyCode.Z))
            Process();
    }

    public void Process()
    {
        float d = 99999f;
        Zitta_TempInteractScript TIC = null;
        for (int i = 0; i < Interacts.Count; i++)
        {
            if ((Interacts[i].transform.position - transform.position).magnitude < d)
            {
                d = (Interacts[i].transform.position - transform.position).magnitude;
                TIC = Interacts[i];
            }
        }
        if (TIC)
            TIC.Process();
    }
}
