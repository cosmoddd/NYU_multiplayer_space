using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NewVoxelAdderScript : NetworkBehaviour
{
    public bool withinValidArea;
    RaycastHit hit;

    Vector3 placePosition;
    Vector3 digPosition;

    public Mesh cubeMesh;

    newMeshHolderScript[] holders;

    public newMeshHolderScript MH;
    public int currentID;

    public int chunkSize = 5;

    int layerMask = 1 << 16; // only against layer 16

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(cubeMesh.vertices.Length);
        holders = GameObject.Find("HOLDERS").GetComponentsInChildren<newMeshHolderScript>();
        MH = holders[0];
        if (!isLocalPlayer)
        {
            GetComponent<NewVoxelAdderScript>().enabled = false;
        }
        if (isLocalPlayer && netId!=1)// if this is not the server / host
        {
            CmdInitMesh();
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdInitMesh()
    {
        Debug.Log("New Player Has joined...");
        for (int i = 0; i < holders.Length; i++)
        {
            if (holders[i].MF.mesh.vertices.Length==0)
            {
                continue;
            }
            RpcInitMesh(holders[i].MF.mesh.vertices, holders[i].MF.mesh.triangles, i);
        }       
    }

    [ClientRpc]
    void RpcInitMesh(Vector3[] verts, int[] tris, int id)
    {
        if (netId!=1 && isLocalPlayer) // not the server
        {
            holders[id].MF.mesh.vertices = verts;
            holders[id].MF.mesh.triangles = tris;
            holders[id].MF.mesh.RecalculateNormals();
            holders[id].updateMesh();
            //Debug.Log(verts.Length);
        }
    }
    public Transform boxOutline;
    public Camera cameraRayCam;

    int camRayCamMask;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        while(cameraRayCam == null)
        {
          GameObject o = GameObject.Find("Camera Variant");
          cameraRayCam = o.GetComponent<Camera>();
            camRayCamMask = cameraRayCam.cullingMask | 1<<16;
            cameraRayCam.cullingMask = camRayCamMask;
        }

        if (cameraRayCam == null) return;

        if (withinValidArea)
        {
            Physics.Raycast(cameraRayCam.ScreenPointToRay(Input.mousePosition), out hit, layerMask); //this is the raycast
            Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider != null)
            {
                placePosition = RoundVectorToInt(hit.point + (hit.normal / 2));
                digPosition = RoundVectorToInt(hit.point - (hit.normal / 2));
                if (boxOutline) boxOutline.position = digPosition;
                if (boxOutline) boxOutline.localScale = new Vector3(1, 1, 1) * 101f;
            }
            else
            {
                if (boxOutline) boxOutline.localScale = new Vector3(1, 1, 1) * .001f;
            }
            if(hit.collider.gameObject.layer != 16)
            {
                if (boxOutline) boxOutline.localScale = new Vector3(1, 1, 1) * .001f;
            }

            if (boxOutline) boxOutline.rotation = Quaternion.Euler(0,0,0);
            if (hit.collider != null && Input.GetKeyDown(KeyCode.Mouse0) && hit.collider.gameObject.layer==16) // add a block
            {
                //AddCubeToMesh(placePosition);
                if (MH.MF.mesh.vertices.Length / 25 > chunkSize)
                {
                    foreach (newMeshHolderScript m in holders)
                    {
                        if (m.MF.mesh.vertices.Length / 25 <= chunkSize)
                        {
                            currentID = m.ID;
                            break;
                        }
                    }
                } // makes each chunk smaller than a set amount

                CmdAddCubeToMesh(placePosition, currentID);
            }

            if (hit.collider != null && hit.collider.tag == "cubes" && Input.GetKeyDown(KeyCode.Mouse1) && hit.collider.gameObject.layer==16) // remove a block
            {
                MH = hit.collider.GetComponent<newMeshHolderScript>();
                //delCubefromMesh(digPosition);
                CmddelCubefromMesh(digPosition, MH.ID);
            }
        }
        else
        {
            if (boxOutline) boxOutline.localScale = new Vector3(1, 1, 1) * .001f;
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdAddCubeToMesh(Vector3 pos, int id)
    {
        RpcAddCubeToMesh(pos, id);
    }
    [ClientRpc]
    void RpcAddCubeToMesh(Vector3 pos, int id)
    {
        AddCubeToMesh(pos, id);
    }

    void AddCubeToMesh(Vector3 pos, int id)
    {
        //print("before");
        //Debug.Log(id);

        MH = holders[id];       
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3[] newVerts = cubeMesh.vertices;
        int[] newTris = cubeMesh.triangles;

        for (int i = 0; i < newVerts.Length; i++)
        {
            newVerts[i] = MH.transform.InverseTransformPoint(pos + newVerts[i]);
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
            triangles.Add(newTris[i] + triLength);
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

        //print("after");
        //Debug.Log(currentID);
        ////Debug.Log(MH.ID);

        MH.updateMesh();
    }


    [Command(ignoreAuthority = true)]
    void CmddelCubefromMesh(Vector3 pos, int id)
    {
        RpcdelCubefromMesh(pos, id);
    }
    [ClientRpc]
    void RpcdelCubefromMesh(Vector3 pos, int id)
    {
        delCubefromMesh(pos,id);
    }

    void delCubefromMesh(Vector3 pos, int id)
    {
        MH = holders[id];
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < MH.MF.mesh.vertices.Length; i++)
        {
            vertices.Add(MH.MF.mesh.vertices[i]);
        }

        //Debug.Log(vertices.IndexOf(pos));
        int centerIndex = vertices.IndexOf(pos);
        vertices.RemoveRange(centerIndex - 8, 25);

        List<int> triangles = new List<int>();
        for (int i = cubeMesh.triangles.Length; i < MH.MF.mesh.triangles.Length; i++)
        {
            triangles.Add(MH.MF.mesh.triangles[i]);
        }

        MH.MF.mesh.triangles = triangles.ToArray();
        MH.MF.mesh.vertices = vertices.ToArray();

        MH.updateMesh();
    }

    Vector3 RoundVectorToInt(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(digPosition,Vector3.one);
    }
}
