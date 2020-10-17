using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_FaceCollider : MonoBehaviour {
    public Zitta_Block Block;
    public int Index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Block.CmdChangeValue(Index);
    }
}
