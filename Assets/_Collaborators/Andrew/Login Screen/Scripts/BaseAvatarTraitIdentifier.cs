using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAvatarTraitIdentifier : MonoBehaviour
{
    public int avatarID; // this is assigned to each prefab, In editor

    public GameObject hatTransform; //this Gameobject has a mesh filter and mesh renderer that can be referenced by the customizer to display the different hat meshes
    public Renderer[] bodyRenderers; //this is an array for people who have multiple meshes / renderers that make up what we would consider the body portionof the avatar

    CharacterCustomizerScript customizer;

    // Start is called before the first frame update
    void Start()
    {
        customizer = GetComponentInParent<CharacterCustomizerScript>();
        customizer.activeAvatar = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (customizer.activeAvatarID != avatarID) //deletes the avatar if it doesnt match with the active avatar id
        {
            Destroy(this.gameObject);
        }
    }
}
