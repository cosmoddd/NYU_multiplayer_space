using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zitta_TempInteraction : MonoBehaviour {
    public static Zitta_TempInteraction Main;
    public Camera C;
    public KeyCode InteractKey;
    public LayerMask Mask;
    public float MaxDistance;

    // Start is called before the first frame update
    void Start()
    {
        Main = this;
        C = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InteractKey) || Input.GetMouseButtonDown(0))
        {
            Ray ray = C.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit, MaxDistance, Mask))
            {
                if (Hit.transform.GetComponent<Zitta_TempInteractScript>())
                {
                    Hit.transform.GetComponent<Zitta_TempInteractScript>().Process();
                }
            }
        }
    }
}
