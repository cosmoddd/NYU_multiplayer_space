using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customizer : MonoBehaviour
{
    Camera mainCam;
    float fov;
    public float rot;
    float lerpRot;

    public string characterName;

    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
    public Color[] bodyColors = new Color[5];

    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 Torso
    public int[] bodyIDs = new int[5];

    // 0 Hat, 1 Head, 2 right foot, 3 left foot
    public Mesh[][] bodyMeshes;

    public Mesh[] hatMeshes;
    public Mesh[] headMeshes;
    public Mesh[] rightFootMeshes;
    public Mesh[] leftFootMeshes;    

    public GameObject[] bodyTransforms;
    public Transform[] TorsoNodes;
    public Renderer[] bodyRenderers;

    public List<float[]> presets;

    public float[] torsoPreset_0;
    public float[] torsoPreset_1;
    public float[] torsoPreset_2;
    public float[] torsoPreset_3;

    public GameObject activeAvatar; //current avatar that you are working on
    public Material defaultHatMaterial; //this is only temporary, eventually this will be coupled with the meshes so that each hat has its own texture
    public SavedAvatarInfoScript savedInfo;

    public NetworkManagerGC manager;
    
    // Start is called before the first frame update
    void Start()
    {
        fov = 42;
        mainCam = Camera.main;
        mainCam.fieldOfView = fov;
        bodyMeshes = new Mesh[][] { hatMeshes, headMeshes, leftFootMeshes, rightFootMeshes};
        LoadTorsoPresets();

        manager = FindObjectOfType<NetworkManagerGC>();
        if(manager)
        {
            Debug.Log(manager);
        }
        else
        {
            Debug.Log("Could not find manager");
        }
    }

    public void SaveData()
    {
        manager.dataMessage.bodyIDs = bodyIDs;
        manager.dataMessage.userName = characterName;
        
        for(int i = 0; i < bodyColors.Length; i++)
        {
            manager.dataMessage.bodyColors[i] = new Vector3(bodyColors[i].r, bodyColors[i].g, bodyColors[i].b);
        }

        manager.DisplayData();
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
        ActiveAvatarTraitAssigner();
        if (characterName != "")
        {
            Zoom();
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
            RotateCharacter();
        }

        //only Temp!!!
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SaveData();
        }
    }

    public void Zoom()
    {
        fov -= Input.GetAxisRaw("Mouse ScrollWheel") * Time.deltaTime * 3000;
        fov = Mathf.Clamp(fov, 21, 42);

        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, fov, Time.deltaTime * 5);
    }

    public void bodyIDChange(int ID, int changeInt)
    {
        // torso is index 4
        if(ID == 4)
        {
            bodyIDs[ID] = Mathf.Clamp(bodyIDs[ID] + changeInt, 0, presets.Count - 1);
        }
        else
        {
            bodyIDs[ID] = Mathf.Clamp(bodyIDs[ID] + changeInt, 0, bodyMeshes[ID].Length - 1);
        }    
    }

    public void ActiveAvatarTraitAssigner()
    {
        foreach (Renderer bodyRenderer in bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColors[4]);
            bodyRenderer.material.SetColor("_Color", bodyColors[4]);
        }

        // loop through body parts and assign meshes and colros accordingly
        for(int i = 0; i < bodyMeshes.Length; i++)
        {
            AssignFromArray(bodyIDs[i], bodyMeshes[i], bodyTransforms[i], bodyColors[i]);
        }

        for (int i = 0; i < TorsoNodes.Length; i++)
        {
            TorsoNodes[i].localScale = Vector3.Lerp(TorsoNodes[i].localScale, Vector3.one * presets[bodyIDs[4]][i],
                Time.deltaTime * 10);
        }
    }

    void AssignFromArray(int ID, Mesh[] meshes, GameObject point, Color color)
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

    public void RotateCharacter()
    {     
        rot -= Input.GetAxisRaw("Mouse X") * Time.deltaTime * 900;
        lerpRot = Mathf.Lerp(lerpRot, rot, Time.deltaTime * 5);

        transform.localRotation = Quaternion.Euler(0, lerpRot, 0);
    }
}
