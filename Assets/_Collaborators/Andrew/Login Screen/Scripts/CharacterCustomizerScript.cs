using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizerScript : MonoBehaviour
{
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

    Camera mainCam;
    float fov;

    public int activeTorsoID;

    public float[] torsoPreset_0;
    public float[] torsoPreset_1;
    public float[] torsoPreset_2;
    public float[] torsoPreset_3;

    public List<float[]> presets;
    // Start is called before the first frame update
    void Start()
    {
        fov = 42;
        mainCam = Camera.main;
        mainCam.fieldOfView = fov;
        bodyColor = Color.grey;
        hatColor = Color.grey;
        headColor = Color.grey;
        footColor = Color.grey;
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
    }


    public void Zoom()
    {
        fov -= Input.GetAxisRaw("Mouse ScrollWheel") * Time.deltaTime * 3000;
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

    void ActiveAvatarTraitAssigner()
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

}
