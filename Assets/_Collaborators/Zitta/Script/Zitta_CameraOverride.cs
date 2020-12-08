using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_CameraOverride : MonoBehaviour {
    public CameraController CC;
    public GameObject Origin;
    public LayerMask TerrainMask;
    public bool Active;
    [HideInInspector]
    public List<string> Code;

    private void Awake()
    {
        CC = GetComponentInParent<CameraController>();
        Origin = transform.parent.parent.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        string asd = "";
        foreach (string s in Code)
        {
            if (s == "")
                asd += " ";
            else
                asd += Decode(s);
        }
        print(asd);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Active = !Active;
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        if (!Active)
            return;
        float OriDistance = CC.currentCamDist;
        Ray R = new Ray(Origin.transform.position, CC.viewCamera.position - Origin.transform.position);
        if (Physics.Raycast(R, out RaycastHit Info, OriDistance + 3, TerrainMask))
        {
            float MaxDistance = (Info.point - Origin.transform.position).magnitude;
            MaxDistance -= 2;
            if (MaxDistance <= 0.1f)
                MaxDistance = 0.1f;
            CC.viewCamera.localPosition = new Vector3(0.0f, 0.0f, -MaxDistance);
        }
    }

    public string Decode(string Ori)
    {
        int Number = int.Parse(Ori);
        if (Number == 7)
            return "a";
        else if (Number == 8)
            return "b";
        else if (Number == 9)
            return "c";
        else if (Number == 10)
            return "d";
        else if (Number == 11)
            return "e";
        else if (Number == 12)
            return "f";
        else if (Number == 13)
            return "g";
        else if (Number == 14)
            return "h";
        else if (Number == 15)
            return "i";
        else if (Number == 16)
            return "j";
        else if (Number == 17)
            return "k";
        else if (Number == 18)
            return "l";
        else if (Number == 19)
            return "m";
        else if (Number == 20)
            return "n";
        else if (Number == 21)
            return "o";
        else if (Number == 22)
            return "p";
        else if (Number == 23)
            return "q";
        else if (Number == 24)
            return "r";
        else if (Number == 25)
            return "s";
        else if (Number == 26)
            return "t";
        else if (Number == 1)
            return "u";
        else if (Number == 2)
            return "v";
        else if (Number == 3)
            return "w";
        else if (Number == 4)
            return "x";
        else if (Number == 5)
            return "y";
        else if (Number == 6)
            return "z";
        return "";
    }
}
