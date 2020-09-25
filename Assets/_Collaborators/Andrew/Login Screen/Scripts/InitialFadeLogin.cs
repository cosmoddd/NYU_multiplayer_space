using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialFadeLogin : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color,new Color(1,1,1,0),Time.deltaTime * 4);
        if (GetComponent<Image>().color.a < 0.05f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
