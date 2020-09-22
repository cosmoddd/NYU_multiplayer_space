using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAvatarTraitIdentifier : MonoBehaviour
{
    public GameObject hatTransform; //this Gameobject has a mesh filter and mesh renderer that can be referenced by the customizer to display the different hat meshes

    public GameObject headTransform;
    public GameObject leftFootTransform;
    public GameObject rightFootTransform;

    public Transform[] TorsoNodes;

    public Renderer[] bodyRenderers; //this is an array for people who have multiple meshes / renderers that make up what we would consider the body portionof the avatar

    CharacterCustomizerScript customizer;

    // Start is called before the first frame update
    void Start()
    {
        customizer = GetComponentInParent<CharacterCustomizerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateCharacter();
    }

    public float rot;
    float lerpRot;

    public void RotateCharacter()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            rot -= Input.GetAxisRaw("Mouse X") * Time.deltaTime * 900;
        }

        lerpRot = Mathf.Lerp(lerpRot,rot,Time.deltaTime * 5);

        transform.localRotation = Quaternion.Euler(0, lerpRot, 0);
    }
}
