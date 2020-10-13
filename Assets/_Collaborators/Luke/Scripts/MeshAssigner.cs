using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAssigner : NetworkBehaviour
{
    // custom struct and class to allow syncing of Vector list
    public struct CustomColor
    {
        public Vector3 color;

        public CustomColor(Vector3 input)
        {
            color = input;
        }
    };

    public class SyncColor : SyncList<CustomColor> {}
    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
    public SyncColor bodyColors = new SyncColor();

    [SyncVar]
    public string characterName;

    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 Torso
    public SyncListInt bodyIDs = new SyncListInt();

    // 0 Hat, 1 Head, 2 right foot, 3 left foot
    public Mesh[][] bodyMeshes;

    // body meshes stored in a scriptable object
    public CharacterMeshData meshData;

    public GameObject[] bodyTransforms;
    public Transform[] TorsoNodes;
    public Renderer[] bodyRenderers;

    public GameObject activeAvatar; //current avatar that you are working on
    public Material defaultHatMaterial; //this is only temporary, eventually this will be coupled with the meshes so that each hat has its own texture

    [HideInInspector]
    public NetworkManagerGC manager;

    public override void OnStartClient()
    {
        AssignAvatarTraits();
    }

    void Awake()
    {
        bodyMeshes = new Mesh[][] { meshData.hatMeshes, meshData.headMeshes, meshData.leftFootMeshes, meshData.rightFootMeshes };
        manager = FindObjectOfType<NetworkManagerGC>();
    }

    public void LoadData(CustomizerData customData)
    {
        characterName = customData.userName;

        foreach (int ID in customData.bodyIDs)
        {
            bodyIDs.Add(ID);
        }

        foreach (Vector3 colorData in customData.bodyColors)
        {
            CustomColor colorStruct = new CustomColor(colorData);
            bodyColors.Add(colorStruct);
        }
    }

    public void AssignAvatarTraits()
    {
        Color bodyColor = new Color(bodyColors[4].color.x, bodyColors[4].color.y, bodyColors[4].color.z);

        foreach (Renderer bodyRenderer in bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColor);
            bodyRenderer.material.SetColor("_Color", bodyColor);
        }

        // loop through body parts and assign meshes and colros accordingly
        for (int i = 0; i < bodyMeshes.Length; i++)
        {
            Color meshColor = new Color(bodyColors[i].color.x, bodyColors[i].color.y, bodyColors[i].color.z);

            AssignFromArray(bodyIDs[i], bodyMeshes[i], bodyTransforms[i], meshColor);
        }

        for (int i = 0; i < TorsoNodes.Length; i++)
        {
            TorsoNodes[i].localScale = Vector3.one * meshData.bodyPresets[bodyIDs[4]].presetValues[i];
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
}
