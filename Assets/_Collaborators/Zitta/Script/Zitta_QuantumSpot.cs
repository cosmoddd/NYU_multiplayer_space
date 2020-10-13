using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_QuantumSpot : NetworkBehaviour {
    public List<GameObject> Points;
    public Collider C2D;
    [Space]
    public bool Active;
    public bool Visible;
    [Space]
    public float VisibleTime;

    private void Awake()
    {
        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active && Zitta_CameraDetection.Main)
        {
            Active = true;
            StartCoroutine("LocalProcess");
        }

        if (isServer)
            VisibleTime -= Time.deltaTime;
    }

    public IEnumerator LocalProcess()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            Visible = IsVisible();
            if (Visible)
                CmdVisibleCommand();
        }
    }

    public bool IsVisible()
    {
        Camera C = Zitta_CameraDetection.Main.C;
        if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(C), C2D.bounds))
            return false;
        foreach (GameObject G in Points)
        {
            RaycastHit[] Hits = Physics.RaycastAll(C.transform.position, direction: G.transform.position - C.transform.position,
                (G.transform.position - C.transform.position).magnitude - 0.1f, ~0, QueryTriggerInteraction.Ignore);
            if (Hits.Length <= 0 || (Hits.Length == 1 && Hits[0].collider.attachedRigidbody))
                return true;
        }
        return false;
    }

    [Command(ignoreAuthority = true)]
    public void CmdVisibleCommand()
    {
        VisibleTime = 0.5f;
    }
}
