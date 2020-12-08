using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridDetectionScript : NetworkBehaviour
{

    RaycastHit hit;

    Vector3 placePosition;
    Vector3 digPosition;

    public Mesh cubeMesh;
    Mesh bigChunk;

    MeshFilter MF;
    MeshRenderer MR;
    MeshHolderScript MH;

    public Vector3[] currentMeshVerts;
    public int[] currentMeshTris;
    public Vector3[] currentMeshNormals;

    Transform cubeOutline;
    MeshFilter outlineFilter;
    MeshRenderer outlineRenderer;

    

    void Start()
    {
        
        MF = GameObject.Find("MeshHolder").GetComponent<MeshFilter>();
        MR = GameObject.Find("MeshHolder").GetComponent<MeshRenderer>();
        MH = GameObject.Find("MeshHolder").GetComponent<MeshHolderScript>();

        bigChunk = new Mesh();
        bigChunk.name = "chunk";
        MF.mesh = bigChunk;

        MH.UpdateMesh();
        triCount = MH.syncedTris.Count;
        localTriCount = MH.MF.mesh.triangles.Length;
         
        outlineFilter = GetComponentInChildren<MeshFilter>();
        outlineRenderer = GetComponentInChildren<MeshRenderer>();
        cubeOutline = outlineFilter.transform;
    
    }



    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space)) { MH.UpdateMesh(); }
        // checkIfSyncListChanged();

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
        if (hit.collider!=null)
        {
            placePosition = RoundVectorToInt(hit.point + (hit.normal / 2));
            digPosition = RoundVectorToInt(hit.point - (hit.normal / 2));
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            CmdAddCubeToMesh(placePosition);
            CmdUpdateMesh();

        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && hit.collider.tag == "cubes")
        {
            CmdRemoveCubeFromMesh(digPosition);
            CmdUpdateMesh();
        }

        checkIfLocalListChanged();
        checkIfSyncListChanged();
        

        outlineRenderer.enabled = hit.collider != null;
        cubeOutline.position = digPosition;
    }

    int triCount;

    void checkIfSyncListChanged()
    {
        if (MH.syncedTris.Count!=triCount)
        {
            // print("THE SYNC LIST HAS CHANGED!");
            MH.UpdateMesh();
            localTriCount = MH.MF.mesh.triangles.Length; //?????
            triCount = MH.syncedTris.Count;
        }
    }

    int localTriCount;

    void checkIfLocalListChanged()
    {
      if (MH.MF.mesh.triangles.Length!= localTriCount)
      {
        CmdUpdateMesh();
        localTriCount = MH.MF.mesh.triangles.Length;
      }
    }

    [Command(ignoreAuthority = true)]
    void CmdUpdateMesh()
    {
        MH.syncedTris.Clear();
        for (int t = 0; t < MH.MF.mesh.triangles.Length; t++)
        {
            MH.syncedTris.Add(MH.MF.mesh.triangles[t]);
        }

        MH.syncedVerts.Clear();
        for (int v = 0; v < MH.MF.mesh.vertices.Length; v++)
        {
            MH.syncedVerts.Add(MH.MF.mesh.vertices[v]);
        }

        // RpcUpdateMesh();
    }

    // [ClientRpc]
    // void RpcUpdateMesh()
    // {
    //     MH.UpdateMesh();
    // }

    [Command(ignoreAuthority = true)]
    void CmdAddCubeToMesh(Vector3 pos)
    {
        checkIfLocalListChanged();
        checkIfSyncListChanged();
        //Destroy(GameObject.Find("MeshHolder").GetComponent<MeshCollider>());
        //
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3[] newVerts = cubeMesh.vertices;
        int[] newTris = cubeMesh.triangles;

        for (int i = 0; i < newVerts.Length; i++)
        {
            newVerts[i] = transform.InverseTransformPoint(pos+newVerts[i]);
            vertices.Add(newVerts[i]);
            //if (newVerts[i] == pos)
            //{
            //    //Debug.Log(cubeMesh.vertices.Length);
            //    //Debug.Log($"center Detected at index {i}");
            //}
        }

        int triLength = MH.MF.mesh.vertices.Length;

        for (int i = 0; i < newTris.Length; i++)
        {
            triangles.Add(newTris[i]+triLength);
        }
       

        for (int i = 0; i < MH.MF.mesh.vertices.Length; i++)
        {
            vertices.Add(MH.MF.mesh.vertices[i]);
        }


        for (int i = 0; i < MH.MF.mesh.triangles.Length; i++)
        {          
            triangles.Add(MH.MF.mesh.triangles[i]);           
        }

        MH.MF.mesh.vertices = vertices.ToArray();
        MH.MF.mesh.triangles = triangles.ToArray();
        MH.MF.mesh.RecalculateNormals();

        checkIfLocalListChanged();
        checkIfSyncListChanged();

        //GameObject.Find("MeshHolder").AddComponent<MeshCollider>();
    }

    [Command(ignoreAuthority = true)]
    void CmdRemoveCubeFromMesh(Vector3 pos)
    {
        checkIfLocalListChanged();
        checkIfSyncListChanged();
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < MH.MF.mesh.vertices.Length; i++)
        {
            vertices.Add(MH.MF.mesh.vertices[i]);
        }

        Debug.Log(vertices.IndexOf(pos));
        int centerIndex = vertices.IndexOf(pos);
        vertices.RemoveRange(centerIndex - 8, 25);

        List<int> triangles = new List<int>();
        for (int i = cubeMesh.triangles.Length; i < MH.MF.mesh.triangles.Length; i++)
        {
            triangles.Add(MH.MF.mesh.triangles[i]);
        }

        MH.MF.mesh.triangles = triangles.ToArray();
        MH.MF.mesh.vertices = vertices.ToArray();

        checkIfLocalListChanged();
        checkIfSyncListChanged();

        // Debug.Log(bigChunk.vertices.Length);
        // Debug.Log(bigChunk.triangles.Length);

        //  CmdUpdateMesh();
        //GameObject.Find("MeshHolder").AddComponent<MeshCollider>();
    }

    private void OnDrawGizmos()
    {       
        if (hit.collider!=null)
        {
            //Gizmos.color = new Color(1,0,0,.5f);
            //Gizmos.DrawCube(placePosition, Vector3.one);
            //Gizmos.DrawWireCube(placePosition, Vector3.one);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(digPosition, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, .1f);
            Gizmos.DrawLine(hit.point, hit.point + hit.normal);
        }
    }

    Vector3 RoundVectorToInt(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }
}
