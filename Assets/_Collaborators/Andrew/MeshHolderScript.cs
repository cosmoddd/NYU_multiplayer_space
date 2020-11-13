using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeshHolderScript : NetworkBehaviour
{
    MeshFilter MF;


    public class SyncTri : SyncList<int> { }
    public SyncTri syncedTris = new SyncTri();

    public class SyncVert : SyncList<Vector3> { }
    public SyncVert syncedVerts = new SyncVert();

    // Start is called before the first frame update
    void Start()
    {
        syncedTris.Callback += OnTrisUpdated;
        MF = GetComponent<MeshFilter>();
    }

    public void UpdateMesh()
    {
        Destroy(GetComponent<MeshCollider>());

        Vector3[] verts = new Vector3[syncedVerts.Count];
        int[] tris = new int[syncedTris.Count];

        for (int v = 0; v < verts.Length; v++)
        {
            verts[v] = syncedVerts[v];

        }

        for (int t = 0; t < tris.Length; t++)
        {
            tris[t] = syncedTris[t];
        }

        MF.mesh.vertices = verts;
        MF.mesh.triangles = tris;

        MF.mesh.RecalculateNormals();
        
        this.gameObject.AddComponent<MeshCollider>();
    }

    void OnTrisUpdated(SyncList<int>.Operation op, int index, int oldInt, int newInt)
    {
        
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                //UpdateMesh();
                // index is where it got added in the list
                // item is the new item
                break;
            case SyncList<int>.Operation.OP_CLEAR:
                //UpdateMesh();
                // list got cleared
                break;
            case SyncList<int>.Operation.OP_INSERT:
                // index is where it got added in the list
                // item is the new item
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                // index is where it got removed in the list
                // item is the item that was removed
                break;
            case SyncList<int>.Operation.OP_SET:
                // index is the index of the item that was updated
                // item is the previous item
                break;
        }
    }
}
