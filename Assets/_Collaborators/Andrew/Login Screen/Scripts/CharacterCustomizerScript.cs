using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CharacterCustomizerScript : NetworkBehaviour
{
    [SyncVar]
    public string characterName; 

    public Color bodyColor;
    public Color hatColor;
    public Color headColor;
    public Color footColor;

    public int activeHatID;
    public Mesh[] hatMeshes;

    public int activeHeadID;
    public Mesh[] headMeshes;

    public int activeFootID;
    public Mesh[] leftFootMeshes;
    public Mesh[] rightFootMeshes;

    public GameObject activeAvatar; //current avatar that you are working on

    public Material defaultHatMaterial; //this is only temporary, eventually this will be coupled with the meshes so that each hat has its own texture

    public SavedAvatarInfoScript savedInfo;

    public float scrollSpeed = 4f;

    Camera mainCam;
    float fov;

    public int activeTorsoID;

    public float[] torsoPreset_0;
    public float[] torsoPreset_1;
    public float[] torsoPreset_2;
    public float[] torsoPreset_3;

    public List<float[]> presets;

    public TextMeshPro playerNameAvatar;
    // Start is called before the first frame update
    void Start()
    {
        fov = 42;
        mainCam = Camera.main;
        mainCam.fieldOfView = fov;
        //bodyColor = Color.grey;
        //hatColor = Color.grey;
        //headColor = Color.grey;
        //footColor = Color.grey;
        LoadTorsoPresets();
    }

    void LoadTorsoPresets()
    {
        presets = new List<float[]>();
        presets.Add(torsoPreset_0);
        presets.Add(torsoPreset_1);
        presets.Add(torsoPreset_2);
        presets.Add(torsoPreset_3);
    }

    // Update is called once per frame
    void Update()
    {
        activeHatID = Mathf.Clamp(activeHatID,0,hatMeshes.Length-1);
        activeHeadID = Mathf.Clamp(activeHeadID, 0, headMeshes.Length - 1);
        activeFootID = Mathf.Clamp(activeFootID, 0, leftFootMeshes.Length - 1);
        activeTorsoID = Mathf.Clamp(activeTorsoID, 0, presets.Count - 1);

        ActiveAvatarTraitAssigner();
        if (characterName != "")
        {
            Zoom();
        }

        //only Temp!!!
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    SaveTraitsToScript();
        //}
    }

    public void assignFromSavedInfo()
    {
        print("assignFromSavedInfo");
        activeHatID = savedInfo.HatMeshID;
        activeHeadID = savedInfo.HeadMeshID;
        activeFootID = savedInfo.FeetMeshID;
        activeTorsoID = savedInfo.TorsoID;

        footColor = new Color(savedInfo.FootColor.x, savedInfo.FootColor.y, savedInfo.FootColor.z,1);
        hatColor = new Color(savedInfo.HatColor.x, savedInfo.HatColor.y, savedInfo.HatColor.z, 1);
        bodyColor = new Color(savedInfo.BodyColor.x, savedInfo.BodyColor.y, savedInfo.BodyColor.z, 1);
        headColor = new Color(savedInfo.HeadColor.x, savedInfo.HeadColor.y, savedInfo.HeadColor.z, 1);
        print("savedInfo.userName");
        playerNameAvatar.text = savedInfo.userName;

    }

    public void Zoom()
    {
        fov -= Input.GetAxisRaw("Mouse ScrollWheel") * Time.deltaTime * scrollSpeed * 1000;
        fov = Mathf.Clamp(fov,21,42);



        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView,fov,Time.deltaTime * 5);
    }

    public void HatIDIncrement(int changeInt)
    {
        activeHatID += changeInt;
        activeHatID = Mathf.Clamp(activeHatID, 0, hatMeshes.Length - 1);
    }

    public void HeadIDIncrement(int changeInt)
    {
        activeHeadID += changeInt;
        activeHeadID = Mathf.Clamp(activeHeadID, 0, headMeshes.Length - 1);
    }

    public void FootIDIncrement(int changeInt)
    {
        activeFootID += changeInt;
        activeFootID = Mathf.Clamp(activeFootID, 0, leftFootMeshes.Length - 1);
    }

    public void TorsoIDIncrement(int changeInt)
    {
        activeTorsoID += changeInt;
        activeTorsoID = Mathf.Clamp(activeTorsoID, 0, presets.Count - 1);
    }

    public void ActiveAvatarTraitAssigner()
    {
        foreach (Renderer bodyRenderer in activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColor);
            bodyRenderer.material.SetColor("_Color", bodyColor);
        }

        AssignFromArray(activeHatID,hatMeshes, activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().hatTransform,hatColor);
        AssignFromArray(activeHeadID,headMeshes, activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().headTransform,headColor);
        AssignFromArray(activeFootID,leftFootMeshes, activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().leftFootTransform,footColor);
        AssignFromArray(activeFootID,rightFootMeshes, activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().rightFootTransform,footColor);


        for (int i = 0; i < activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().TorsoNodes.Length; i++)
        {
            activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().TorsoNodes[i].localScale = Vector3.Lerp(activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().TorsoNodes[i].localScale, 
                                                                                                           Vector3.one * presets[activeTorsoID][i],
                                                                                                           Time.deltaTime * 10);
        }
    }

    void AssignFromArray(int ID, Mesh[] meshes, GameObject point,Color color)
    {
        if (ID > 0)
        {
            point.GetComponent<MeshFilter>().mesh = meshes[ID];
        }
        else
        {
            point.GetComponent<MeshFilter>().mesh = null;
        }

        point.GetComponent<Renderer>().material.SetColor("_Color", color);
        point.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
    }

    public void SaveTraitsToScript()
    {
        savedInfo.userName = characterName;

        savedInfo.HeadMeshID = activeHeadID;
        savedInfo.FeetMeshID = activeFootID;
        savedInfo.HatMeshID = activeHatID;
        savedInfo.TorsoID = activeTorsoID;

        savedInfo.HeadColor = new Vector3(headColor.r, headColor.g,headColor.b);
        savedInfo.BodyColor = new Vector3(bodyColor.r, bodyColor.g, bodyColor.b);
        savedInfo.FootColor = new Vector3(footColor.r, footColor.g, footColor.b);
        savedInfo.HatColor = new Vector3(hatColor.r, hatColor.g, hatColor.b);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        assignFromSavedInfo();
    }

}
