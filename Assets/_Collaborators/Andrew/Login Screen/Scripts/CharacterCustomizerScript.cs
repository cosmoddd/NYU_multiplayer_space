using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizerScript : MonoBehaviour
{
    public string characterName; 

    public GameObject[] BaseAvatarGameObjects; //array of avatar gameobjects with hat gameobjects and pertenint compoenents as children
    public int activeAvatarID; 
    public Color bodyColor;
    public Color hatColor;

    public int activeHatID;
    public Mesh[] hatMeshes;

    public GameObject activeAvatar; //current avatar that you are working on

    public Material defaultHatMaterial; //this is only temporary, eventually this will be coupled with the meshes so that each hat has its own texture

    // Start is called before the first frame update
    void Start()
    {
        activeAvatarID = 0;
        bodyColor = Color.white;
        hatColor = Color.white;

        Instantiate(BaseAvatarGameObjects[activeAvatarID],transform); //spawns avatar with ID of 0 as a child of this gameobject (the default avatar position)
    }

    // Update is called once per frame
    void Update()
    {
        activeAvatarID = Mathf.Clamp(activeAvatarID,0,BaseAvatarGameObjects.Length-1); //makes it so that the ID always is referencing an existing entry in the array
        activeHatID = Mathf.Clamp(activeHatID,0,hatMeshes.Length-1);

        //currently the IDs do NOT wrap around from the last entry to the first

        BaseAvatarAssigner(); //assigns the gameobject with the base avatar mesh based on the activeavatarID
        ActiveAvatarTraitAssigner();
    }

    void BaseAvatarAssigner()
    {
        if (activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().avatarID != activeAvatarID) //checks to see if the avatar is matching the ID, if not it deletes it and spawns the appropriate one
        {
            Destroy(activeAvatar);
            Instantiate(BaseAvatarGameObjects[activeAvatarID], transform);
        }
    }

    void ActiveAvatarTraitAssigner()
    {
        foreach (Renderer bodyRenderer in activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().bodyRenderers)//sets the body color
        {
            bodyRenderer.material.SetColor("_BaseColor", bodyColor);
            bodyRenderer.material.SetColor("_Color", bodyColor);
        }

        if (activeHatID>0) // sets the hat mesh, with zero being no hat
        {
            activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().hatTransform.GetComponent<MeshFilter>().mesh = hatMeshes[activeHatID];
        }
        else
        {
            activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().hatTransform.GetComponent<MeshFilter>().mesh = null;
        }


        activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().hatTransform.GetComponent<MeshRenderer>().material = defaultHatMaterial; //temporary, after the hats are designed there will be custom materials for them
        activeAvatar.GetComponent<BaseAvatarTraitIdentifier>().hatTransform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", hatColor);//sets the hat color
    }
}
