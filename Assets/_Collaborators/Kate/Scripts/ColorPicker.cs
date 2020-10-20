using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{

    public Camera cam;
    public Color selectedColor;
    public float rayDistance;

   // public Image image;

    //public CharacterCustomizationUI customizerUI;
    public CharacterCustomizationUITabs customizerUI;



    


    // Update is called once per frame
    void Update()
    {
        
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position,cam.ScreenPointToRay(Input.mousePosition).direction * rayDistance , Color.red);

        if(Input.GetMouseButton(0) && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, rayDistance))
        {
            if(!hit.transform.CompareTag("ColorPicker")) return;

            // print("click");

            Renderer thisRenderer = hit.transform.GetComponent<MeshRenderer>();

            Texture2D text = (Texture2D) thisRenderer.material.mainTexture;

            Vector2 pixelUV = hit.textureCoord; //UV coord on object hit with racyast

            selectedColor = text.GetPixelBilinear(pixelUV.x, pixelUV.y);

            customizerUI.SetColor(selectedColor);


        }
    }


}
