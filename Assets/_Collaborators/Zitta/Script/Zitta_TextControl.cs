using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_TextControl : MonoBehaviour {
    public static Zitta_TextControl Main;
    public GameObject TextRenderer;
    public GameObject TempObject;

    public void Awake()
    {
        Main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject G = Instantiate(TextRenderer);
            G.GetComponent<Zitta_TextRenderer>().Ini("Something", TempObject, 2f, 2f, null);
        }
    }
}
