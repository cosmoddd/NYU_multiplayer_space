using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Zitta_Block : NetworkBehaviour {
    [SyncVar] public int DirectionI;
    [SyncVar] public int DirectionII;
    [SyncVar] public int DirectionIII;
    [SyncVar] public int DirectionIV;
    [SyncVar] public int DirectionV;
    [SyncVar] public int DirectionVI;
    public List<GameObject> Faces;
    public List<GameObject> EmptyFaces;
    public GameObject ActiveMesh;
    public GameObject DisableMesh;
    public Rigidbody Rig;
    public float Scale;
    public float MaxLaunchTime;
    public float CurrentLaunchTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaces();

        if (!isServer)
            return;
        if (CurrentLaunchTime >= 0)
        {
            CurrentLaunchTime -= Time.deltaTime;
            Vector3 V = GetForce();
            Rig.AddForce(transform.TransformVector(V) * Scale);
        }
    }

    public void UpdateFaces()
    {
        UpdateFace(1, DirectionI);
        UpdateFace(2, DirectionII);
        UpdateFace(3, DirectionIII);
        UpdateFace(4, DirectionIV);
        UpdateFace(5, DirectionV);
        UpdateFace(6, DirectionVI);
        if (CurrentLaunchTime > 0)
        {
            ActiveMesh.SetActive(true);
            DisableMesh.SetActive(false);
        }
        else
        {
            ActiveMesh.SetActive(false);
            DisableMesh.SetActive(true);
        }
    }

    public void UpdateFace(int Index, int Value)
    {
        GameObject Face = Faces[Index];
        GameObject EmptyFace = EmptyFaces[Index];
        if (Value == 0)
        {
            EmptyFace.SetActive(true);
            Face.SetActive(false);
        }
        else
        {
            Face.SetActive(true);
            EmptyFace.SetActive(false);
        }

        if (Index == 1 || Index == 6)
        {
            if (Value == 1)
                Face.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (Value == 2)
                Face.transform.localEulerAngles = new Vector3(0, 90, 0);
            else if (Value == 3)
                Face.transform.localEulerAngles = new Vector3(0, 180, 0);
            else if (Value == 4)
                Face.transform.localEulerAngles = new Vector3(0, 270, 0);
        }
        else if (Index == 2)
        {
            if (Value == 1)
                Face.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (Value == 2)
                Face.transform.localEulerAngles = new Vector3(0, 0, -90);
            else if (Value == 3)
                Face.transform.localEulerAngles = new Vector3(0, 0, -180);
            else if (Value == 4)
                Face.transform.localEulerAngles = new Vector3(0, 0, -270);
        }
        else if (Index == 3)
        {
            if (Value == 1)
                Face.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (Value == 2)
                Face.transform.localEulerAngles = new Vector3(-90, 0, 0);
            else if (Value == 3)
                Face.transform.localEulerAngles = new Vector3(-180, 0, 0);
            else if (Value == 4)
                Face.transform.localEulerAngles = new Vector3(-270, 0, 0);
        }
        else if (Index == 4)
        {
            if (Value == 1)
                Face.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (Value == 2)
                Face.transform.localEulerAngles = new Vector3(0, 0, 90);
            else if (Value == 3)
                Face.transform.localEulerAngles = new Vector3(0, 0, 190);
            else if (Value == 4)
                Face.transform.localEulerAngles = new Vector3(0, 0, 270);
        }
        else if (Index == 5)
        {
            if (Value == 1)
                Face.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (Value == 2)
                Face.transform.localEulerAngles = new Vector3(90, 0, 0);
            else if (Value == 3)
                Face.transform.localEulerAngles = new Vector3(180, 0, 0);
            else if (Value == 4)
                Face.transform.localEulerAngles = new Vector3(270, 0, 0);
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdChangeValue(int Index)
    {
        ChangeValue(Index);
    }

    public void ChangeValue(int Index)
    {
        print("ChangeValue " + Index);
        if (CurrentLaunchTime > 0)
            return;
        if (Index == 1)
        {
            DirectionI++;
            if (DirectionI > 4)
                DirectionI = 0;
        }
        else if (Index == 2)
        {
            DirectionII++;
            if (DirectionII > 4)
                DirectionII = 0;
        }
        else if (Index == 3)
        {
            DirectionIII++;
            if (DirectionIII > 4)
                DirectionIII = 0;
        }
        else if (Index == 4)
        {
            DirectionIV++;
            if (DirectionIV > 4)
                DirectionIV = 0;
        }
        else if (Index == 5)
        {
            DirectionV++;
            if (DirectionV > 4)
                DirectionV = 0;
        }
        else if (Index == 6)
        {
            DirectionVI++;
            if (DirectionVI > 4)
                DirectionVI = 0;
        }
    }

    public void Interact()
    {
        CmdLaunch();
    }

    [Command(ignoreAuthority = true)]
    public void CmdLaunch()
    {
        Launch();
    }

    public void Launch()
    {
        // print("Launch");
        if (CurrentLaunchTime > 0)
            return;
        CurrentLaunchTime = MaxLaunchTime;
    }

    public Vector3 GetForce()
    {
        Vector3 T = new Vector3();
        if (DirectionI != 0)
        {
            if (DirectionI == 1)
                T += new Vector3(0, 0, 1);
            else if (DirectionI == 2)
                T += new Vector3(1, 0, 0);
            else if (DirectionI == 3)
                T += new Vector3(0, 0, -1);
            else if (DirectionI == 4)
                T += new Vector3(-1, 0, 0);
        }
        if (DirectionII != 0)
        {
            if (DirectionII == 1)
                T += new Vector3(0, 1, 0);
            else if (DirectionII == 2)
                T += new Vector3(1, 0, 0);
            else if (DirectionII == 3)
                T += new Vector3(0, -1, 0);
            else if (DirectionII == 4)
                T += new Vector3(-1, 0, 0);
        }
        if (DirectionIII != 0)
        {
            if (DirectionIII == 1)
                T += new Vector3(0, 1, 0);
            else if (DirectionIII == 2)
                T += new Vector3(0, 0, -1);
            else if (DirectionIII == 3)
                T += new Vector3(0, -1, 0);
            else if (DirectionIII == 4)
                T += new Vector3(0, 0, 1);
        }
        if (DirectionIV != 0)
        {
            if (DirectionIV == 1)
                T += new Vector3(0, -1, 0);
            else if (DirectionIV == 2)
                T += new Vector3(1, 0, 0);
            else if (DirectionIV == 3)
                T += new Vector3(0, 1, 0);
            else if (DirectionIV == 4)
                T += new Vector3(-1, 0, 0);
        }
        if (DirectionV != 0)
        {
            if (DirectionV == 1)
                T += new Vector3(0, -1, 0);
            else if (DirectionV == 2)
                T += new Vector3(0, 0, -1);
            else if (DirectionV == 3)
                T += new Vector3(0, 1, 0);
            else if (DirectionV == 4)
                T += new Vector3(0, 0, 1);
        }
        if (DirectionVI != 0)
        {
            if (DirectionVI == 1)
                T += new Vector3(0, 0, -1);
            else if (DirectionVI == 2)
                T += new Vector3(-1, 0, 0);
            else if (DirectionVI == 3)
                T += new Vector3(0, 0, 1);
            else if (DirectionVI == 4)
                T += new Vector3(1, 0, 0);
        }
        return T;
    }
}

public enum BlockDirection
{
    Front,
    Back,
    Left,
    Right
}
