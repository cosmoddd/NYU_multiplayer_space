using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class newMeshHolderScript : NetworkBehaviour
{
    public MeshFilter MF;
    public int ID;
    // Start is called before the first frame update
    void Start()
    {
        MF = GetComponent<MeshFilter>();
        MF.mesh = new Mesh();

        for (int i = 0; i < transform.parent.GetComponentsInChildren<newMeshHolderScript>().Length; i++)
        {
            if (this.gameObject == transform.parent.GetComponentsInChildren<newMeshHolderScript>()[i].gameObject)
            {
                ID = i;
                break;
            }
        }

    }

    public void updateMesh()
    {
        if (GetComponent<MeshCollider>())
        {
            Destroy(GetComponent<MeshCollider>());
        }


        this.gameObject.AddComponent<MeshCollider>();
    }


}
