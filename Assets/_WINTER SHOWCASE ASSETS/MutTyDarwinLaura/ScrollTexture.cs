using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
	public Vector2 scrollRate;

    private Renderer _renderer;
    public Material thisMaterial;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        thisMaterial = _renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = Time.time * scrollRate;
        thisMaterial.SetTextureOffset("_BaseMap", offset);
    }
}
