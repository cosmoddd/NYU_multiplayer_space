using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{

    public float rayDistance;
    public Color selectedColor;


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayDistance))
        {
            Renderer thisRenderer = hit.transform.GetComponent<MeshRenderer>();

            Texture2D text = (Texture2D) thisRenderer.material.mainTexture;

            Vector2 pixelUV = hit.textureCoord; //UV coord on object hit with racyast

            selectedColor = text.GetPixelBilinear(pixelUV.x, pixelUV.y);


        }
    }
}
