using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDetectionScript : MonoBehaviour
{

    RaycastHit hit;

    Vector3 placePosition;
    Vector3 digPosition;

    public Mesh cubeMesh;
    Mesh bigChunk;

    MeshFilter MF;
    MeshRenderer MR;

    public Vector3[] currentMeshVerts;
    public int[] currentMeshTris;
    public Vector3[] currentMeshNormals;

    void Start()
    {
        
        MF = GetComponent<MeshFilter>();
        MR = GetComponent<MeshRenderer>();

        bigChunk = new Mesh();
        bigChunk.name = "chunk";
        MF.mesh = bigChunk;
    }

    void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
        if (hit.collider!=null)
        {
            placePosition = RoundVectorToInt(hit.point + (hit.normal / 2));
            digPosition = RoundVectorToInt(hit.point - (hit.normal / 2));
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            addCubeToMesh(placePosition);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && hit.collider.tag == "cubes")
        {
            removeCubeFromMesh(digPosition);
        }

        currentMeshVerts = MF.mesh.vertices;
        currentMeshTris = MF.mesh.triangles;
        currentMeshNormals = MF.mesh.normals;
    }

    void addCubeToMesh(Vector3 pos)
    {
        Destroy(GetComponent<MeshCollider>());
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

        int triLength = bigChunk.vertices.Length;

        for (int i = 0; i < newTris.Length; i++)
        {
            triangles.Add(newTris[i]+triLength);
        }

        

        for (int i = 0; i < bigChunk.vertices.Length; i++)
        {
            vertices.Add(bigChunk.vertices[i]);
        }


        for (int i = 0; i < bigChunk.triangles.Length; i++)
        {          
            triangles.Add(bigChunk.triangles[i]);           
        }

        bigChunk.vertices = vertices.ToArray();
        bigChunk.triangles = triangles.ToArray();
        bigChunk.RecalculateNormals();


        //for (int i = 0; i < cubeMesh.vertices.Length; i++)
        //{
        //    Debug.Log(cubeMesh.vertices[i]);
        //}
        //Debug.Log(pos);
        //for (int i = 0; i < bigChunk.vertices.Length; i++)
        //{
        //    Debug.Log(bigChunk.vertices[i]);
        //}

        //
        this.gameObject.AddComponent<MeshCollider>();
    }

    void removeCubeFromMesh(Vector3 pos)
    {
        Destroy(GetComponent<MeshCollider>());

        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < bigChunk.vertices.Length; i++)
        {
            vertices.Add(bigChunk.vertices[i]);
        }

        Debug.Log(vertices.IndexOf(pos));
        int centerIndex = vertices.IndexOf(pos);
        vertices.RemoveRange(centerIndex - 8, 25);

        List<int> triangles = new List<int>();
        for (int i = cubeMesh.triangles.Length; i < bigChunk.triangles.Length; i++)
        {
            triangles.Add(bigChunk.triangles[i]);
        }

        bigChunk.triangles = triangles.ToArray();
        bigChunk.vertices = vertices.ToArray();

        Debug.Log(bigChunk.vertices.Length);
        Debug.Log(bigChunk.triangles.Length);

        this.gameObject.AddComponent<MeshCollider>();
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
