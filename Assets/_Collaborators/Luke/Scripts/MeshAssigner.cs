using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAssigner : NetworkBehaviour
{
    // custom struct and class to allow syncing of trait lists
    public struct TraitData
    {
        public Vector3 color;
        public int bodyID;

        public TraitData(Vector3 inColor, int ID)
        {
            color = inColor;
            bodyID = ID;
        }
    };

    public class SyncTrait : SyncList<TraitData> {}
    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 Torso
    public SyncTrait bodyTraits = new SyncTrait();

    [SyncVar]
    public string userName;

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
        manager = FindObjectOfType<NetworkManagerGC>();
    }

    public void LoadData(CustomizerData customData)
    {
        userName = customData.userName;

        for(int i = 0; i < customData.bodyIDs.Length; i++)
        {
            TraitData traitStruct = new TraitData(customData.bodyColors[i], customData.bodyIDs[i]);
            bodyTraits.Add(traitStruct);
        }
    }

    public void AssignAvatarTraits()
    {
        Color bodyColor = new Color(bodyTraits[4].color.x, bodyTraits[4].color.y, bodyTraits[4].color.z);

        foreach (Renderer bodyRenderer in bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColor);
            bodyRenderer.material.SetColor("_Color", bodyColor);
        }

        // loop through body parts and assign meshes and colors accordingly
        for (int i = 0; i < meshData.bodyMeshes.Length; i++)
        {
            Color meshColor = new Color(bodyTraits[i].color.x, bodyTraits[i].color.y, bodyTraits[i].color.z);
            AssignFromArray(bodyTraits[i].bodyID, meshData.bodyMeshes[i].meshes, bodyTransforms[i], meshColor);
        }

        for (int i = 0; i < TorsoNodes.Length; i++)
        {
            TorsoNodes[i].localScale = Vector3.one * meshData.bodyPresets[bodyTraits[4].bodyID].presetValues[i];
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
