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

    int trisCount;
    // Start is called before the first frame update
    void Start()
    {
        syncedTris.Callback += OnTrisUpdated;
        MF = GetComponent<MeshFilter>();
        trisCount = syncedTris.Count;

    }

    public void UpdateSyncedMesh(Mesh bigChunk)
    {
        print("Client is calculating the sync'd mesh via BigCHUNK.");
        syncedTris.Clear();
        for (int t = 0; t < bigChunk.triangles.Length; t++)
        {
            syncedTris.Add(bigChunk.triangles[t]);
        }

        syncedVerts.Clear();
        for (int v = 0; v < bigChunk.vertices.Length; v++)
        {
            syncedVerts.Add(bigChunk.vertices[v]);
        }

        RpcUpdateMesh();
    }
    
    [ClientRpc]
    public void RpcUpdateMesh()
    {
      StartCoroutine(UpdateMeshCoroutine());
    }

    public void InitMesh()
    {
      StartCoroutine(UpdateMeshCoroutine());
    }

    IEnumerator UpdateMeshCoroutine()
    {
      // wait until the count is actually different
      while(trisCount == syncedTris.Count)
      {
        yield return null;
      }

      print("Updating the mesh itself.");

        Destroy(GetComponent<MeshCollider>());

        Vector3[] verts = new Vector3[syncedVerts.Count];
        int[] tris = new int[syncedTris.Count];
        MF.mesh.Clear();

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
        this.gameObject.GetComponent<MeshCollider>().enabled = false;
        this.gameObject.GetComponent<MeshCollider>().enabled = true;
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
