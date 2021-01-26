using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;

public enum Role  {Professor, Staff, MFA, BFA, NON}
public class LocalSaveData : MonoBehaviour
{
    string local_userName;
     int local_HeadMeshID;
     int local_FeetMeshID;

     int local_HatMeshID;
    //int local_TorsoID;
   
    float[] local_TorsoIDs;

    float[] tmpTorsoIDs = {1,1,1,1,1,1,1,1};

    Vector4 local_BodyColor;
    Vector4 local_HeadColor;
    Vector4 local_HatColor;
     Vector4 local_FootColor;

    public Customizer localCustomizer;

    public MeshAssigner localAssigner;

    int local_role;

    void Start()
    {
      if (localCustomizer && ES3.FileExists("saveInfo.es3"))
      {
        print("file exists.  loading data");
        LoadData();
        SetCustomizer();
        return;
      }
      else if(localAssigner && ES3.FileExists("saveInfo.es3"))
        {
            LoadData();
        }

      // print("file doesn't exist");
    }

    void LoadData()
    {
        //load info from save script, assign to default values if it didnt exsist before

       local_userName = ES3.Load("username", "saveInfo.es3", "" ); //key, filepath, default value

       local_HeadMeshID = ES3.Load("headID", "saveInfo.es3", 0 ); //key, filepath, default value

       local_FeetMeshID = ES3.Load("footID", "saveInfo.es3", 0 ); //key, filepath, default value

       local_HatMeshID = ES3.Load("hatID", "saveInfo.es3", 0 ); //key, filepath, default value

       //local_TorsoID = ES3.Load("torsoID", "saveInfo.es3", 0 ); //key, filepath, default value
       local_TorsoIDs = ES3.Load("torsoIDs", "saveInfo.es3", tmpTorsoIDs);

        local_BodyColor = ES3.Load("bodyColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_HeadColor = ES3.Load("headColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_HatColor = ES3.Load("hatColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_FootColor = ES3.Load("footColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value
        //headID footID hatID torsoID bodyColor headColor hatColor footColor

        local_role = ES3.Load("roleID", "saveInfo.es3", (int) Role.NON); //load  role ID, set to none as default

    }

    void SetCustomizer()
    {
        Debug.Log("SetCustomizer");
         // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
        localCustomizer.bodyIDs[0] = local_HatMeshID;

        Color tmpColor = new Color(local_HatColor.x, local_HatColor.y, local_HatColor.z, local_HatColor.w);
        localCustomizer.bodyColors[0] = tmpColor;

        localCustomizer.bodyIDs[1] = local_HeadMeshID;
        localCustomizer.bodyColors[1] = local_HeadColor;

        localCustomizer.bodyIDs[2] = local_FeetMeshID;
        localCustomizer.bodyIDs[3] = local_FeetMeshID;
        localCustomizer.bodyColors[2] = local_FootColor;
        localCustomizer.bodyColors[3] = local_FootColor;

        //localCustomizer.bodyIDs[4] = local_TorsoID;
        localCustomizer.SetTorsoNodes(local_TorsoIDs);
        localCustomizer.bodyColors[4] = local_BodyColor;

        localCustomizer.userName = local_userName;

        localCustomizer.nyuRole = (Role)local_role;
    }

    public void SaveUsername()
    {
        ES3.Save("username", localCustomizer.userName, "saveInfo.es3");
    }
/*
    public void AssignerSaveData()
    {
        //headID footID hatID torsoID bodyColor headColor hatColor footColor
        // ES3.Save("username", localCustomizer.userName, "saveInfo.es3");  *NEED TO DO THIS SOMEWHERE ELSE*
        ES3.Save("headID", localAssigner.bodyTraits[1].bodyID, "saveInfo.es3");
        ES3.Save("footID", localAssigner.bodyTraits[2].bodyID, "saveInfo.es3");
        ES3.Save("hatID", localAssigner.bodyTraits[0].bodyID, "saveInfo.es3");
        //ES3.Save("torsoID", localCustomizer.bodyIDs[4], "saveInfo.es3");  **NO Torso ID*
    }
    */

    public void SaveData()
    {
        //headID footID hatID torsoID bodyColor headColor hatColor footColor
       // ES3.Save("username", localCustomizer.userName, "saveInfo.es3");  *NEED TO DO THIS SOMEWHERE ELSE*
        ES3.Save("headID", localCustomizer.bodyIDs[1], "saveInfo.es3");
        ES3.Save("footID", localCustomizer.bodyIDs[2], "saveInfo.es3");
        ES3.Save("hatID", localCustomizer.bodyIDs[0], "saveInfo.es3");
        //ES3.Save("torsoID", localCustomizer.bodyIDs[4], "saveInfo.es3");  **NO Torso ID*

        Vector4 bbodyColor = localCustomizer.bodyColors[4];
        ES3.Save("bodyColor",  bbodyColor, "saveInfo.es3");

        Vector4 bheadColor = localCustomizer.bodyColors[1];
        ES3.Save("headColor", bheadColor, "saveInfo.es3");

        Vector4 bhatColor = localCustomizer.bodyColors[0];
        ES3.Save("hatColor", bhatColor,"saveInfo.es3");

         Vector4 bfootColor = localCustomizer.bodyColors[2];
        ES3.Save("footColor", bfootColor, "saveInfo.es3");

        //local_TorsoIDs = ES3.Load("torsoIDs", "saveInfo.es3", tmpTorsoIDs);
        ES3.Save("torsoIDs",localCustomizer.torsoNodeScales, "saveInfo.es3");

         ES3.Save("roleID", (int)localCustomizer.nyuRole, "saveInfo.es3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
