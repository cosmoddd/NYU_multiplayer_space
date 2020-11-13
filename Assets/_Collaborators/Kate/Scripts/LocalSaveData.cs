using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;

public class LocalSaveData : MonoBehaviour
{
    string local_userName;
     int local_HeadMeshID;
     int local_FeetMeshID;

     int local_HatMeshID;
    int local_TorsoID;

    Vector4 local_BodyColor;
    Vector4 local_HeadColor;
    Vector4 local_HatColor;
     Vector4 local_FootColor;

    public Customizer localCustomizer;

    void Start()
    {
        LoadData();
        SetCustomizer();
    }

    void LoadData()
    {
        //load info from save script, assign to default values if it didnt exsist before

       local_userName = ES3.Load("username", "saveInfo.es3", "" ); //key, filepath, default value

       local_HeadMeshID = ES3.Load("headID", "saveInfo.es3", 0 ); //key, filepath, default value

       local_FeetMeshID = ES3.Load("footID", "saveInfo.es3", 0 ); //key, filepath, default value

       local_HatMeshID = ES3.Load("hatID", "saveInfo.es3", 0 ); //key, filepath, default value

       local_TorsoID = ES3.Load("torsoID", "saveInfo.es3", 0 ); //key, filepath, default value

        local_BodyColor = ES3.Load("bodyColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_HeadColor = ES3.Load("headColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_HatColor = ES3.Load("hatColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value

        local_FootColor = ES3.Load("footColor", "saveInfo.es3", Vector4.zero ); //key, filepath, default value
        //headID footID hatID torsoID bodyColor headColor hatColor footColor

    }

    void SetCustomizer()
    {
        Debug.Log("SetCustomizer");
         // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
        localCustomizer.bodyIDs[0] = local_HatMeshID;
        localCustomizer.bodyColors[0] = local_HatColor;

        localCustomizer.bodyIDs[1] = local_HeadMeshID;
        localCustomizer.bodyColors[1] = local_HeadColor;

        localCustomizer.bodyIDs[2] = local_FeetMeshID;
        localCustomizer.bodyIDs[3] = local_FeetMeshID;
        localCustomizer.bodyColors[2] = local_FootColor;
        localCustomizer.bodyColors[3] = local_FootColor;

        localCustomizer.bodyIDs[4] = local_TorsoID;
        localCustomizer.bodyColors[4] = local_BodyColor;


    }

    void SetUsername()
    {
        //unsure how i will do this
    }

    public void SaveData()
    {
        //headID footID hatID torsoID bodyColor headColor hatColor footColor
        ES3.Save("username", localCustomizer.userName, "saveInfo.es3");
        ES3.Save("headID", localCustomizer.bodyIDs[1], "saveInfo.es3");
        ES3.Save("footID", localCustomizer.bodyIDs[2], "saveInfo.es3");
        ES3.Save("hatID", localCustomizer.bodyIDs[0], "saveInfo.es3");
        ES3.Save("torsoID", localCustomizer.bodyIDs[4], "saveInfo.es3");
        ES3.Save("bodyColor", localCustomizer.bodyColors[4], "saveInfo.es3");
        ES3.Save("headColor", localCustomizer.bodyColors[1], "saveInfo.es3");
        ES3.Save("hatColor", localCustomizer.bodyColors[0],"saveInfo.es3");
        ES3.Save("footColor", localCustomizer.bodyColors[2], "saveInfo.es3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
