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
    }

    // Update is called once per frame
    void Update()
    {
        activeHatID = Mathf.Clamp(activeHatID,0,hatMeshes.Length-1);
        activeHeadID = Mathf.Clamp(activeHeadID, 0, headMeshes.Length - 1);
        activeFootID = Mathf.Clamp(activeFootID, 0, leftFootMeshes.Length - 1);

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
