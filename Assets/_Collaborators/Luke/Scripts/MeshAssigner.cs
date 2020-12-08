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

    // custom struct for creating SyncList for torso scales
    public struct TorsoData
    {
        public float scale;
        
        public TorsoData(float scale)
        {
            this.scale = scale;
        }
    };

    public struct LoginData
    {
      public string[] tags;
      public string nameTag;
      public string password;

      public bool PlayerIs(string tag)
      {
        foreach(string _tag in tags)  if(_tag == tag ) return true;
        return false;
      }
      

      public LoginData(string e, string p, string[] t)
      {
        tags = t;
        nameTag = e;
        password = p;
        addEmojisAndColor();
      }

      void addEmojisAndColor()
      {
        //go by order of importance

        //Moderator Emoji
        if(PlayerIs("Moderator"))
        {
          nameTag = "<sprite index=0>"   + " " + nameTag;
        }

        //colors
        if(PlayerIs("MFA"))
        {
          nameTag = "<#33E9FF>" + nameTag; //TEAL-BLUE
        }
        else if(PlayerIs("BFA"))
        {
          nameTag = "<#33FF83>"  + nameTag; //TEAL-GREEN
        }else if(PlayerIs("Professor"))
        {
          nameTag =  "<#B833FF>"  + nameTag; //PURPLE
        }else if(PlayerIs("Staff"))
        {
          nameTag =  "<#FF338A>"  + nameTag; //PINK
        }



      }
    }


    // custom class
    public class SyncTrait : SyncList<TraitData> {}
    public class SyncTorso : SyncList<TorsoData> {}
    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 body
    // 0 Hat, 1 Head, 2 right foot, 3 left foot, 4 Torso
    public SyncTrait bodyTraits = new SyncTrait();
    public SyncTorso torsoScales = new SyncTorso();

    [SyncVar]
    public string userName;

    public LoginData loginInfo;


    // body meshes stored in a scriptable object
    public CharacterMeshData meshData;

    public GameObject[] bodyTransforms;
    public Transform[] TorsoNodes;
    public Renderer[] bodyRenderers;

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
        loginInfo = new LoginData(customData.email,customData.password,customData.tags);


        for(int i = 0; i < customData.bodyIDs.Length; i++)
        {
            TraitData traitStruct = new TraitData(customData.bodyColors[i], customData.bodyIDs[i]);
            bodyTraits.Add(traitStruct);
        }

        for (int i = 0; i < customData.torsoScales.Length; i++)
        {
            TorsoData torsoData = new TorsoData(customData.torsoScales[i]);
            torsoScales.Add(torsoData);
        }

        userName = loginInfo.nameTag;
    }


    public void AssignAvatarTraits()
    {
        Color bodyColor = new Color(bodyTraits[4].color.x, bodyTraits[4].color.y, bodyTraits[4].color.z);

        foreach (Renderer bodyRenderer in bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColor);
            bodyRenderer.material.SetColor("_Color", bodyColor);
            bodyRenderer.material.SetColor("_EmissionColor", bodyColor * Color.grey);
        }

        // loop through body parts and assign meshes and colors accordingly
        for (int i = 0; i < meshData.bodyMeshes.Length; i++)
        {
            Color meshColor = new Color(bodyTraits[i].color.x, bodyTraits[i].color.y, bodyTraits[i].color.z);
            AssignFromArray(bodyTraits[i].bodyID, meshData.bodyMeshes[i].meshes, bodyTransforms[i], meshColor);
        }

        for (int i = 0; i < TorsoNodes.Length; i++)
        {
            //TODO fix to work with slider presets
            TorsoNodes[i].localScale = Vector3.one * torsoScales[i].scale;
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
        point.GetComponent<Renderer>().material.SetColor("_EmissionColor", color * Color.grey); // add a touch of emission
    }
}
