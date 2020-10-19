using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Mirror;
using UnityAtoms.BaseAtoms;

public class Z_Interactor : MonoBehaviour {
    public static Z_Interactor Main;
    public Camera C;
    public KeyCode InteractKey;
    public LayerMask Mask;
    public float MaxDistance;

    public VoidEvent raycastHoverEnter;
    public VoidEvent raycastHoverExit;
    
    public bool hovering = false;
    // Start is called before the first frame update
//   public override void OnStartClient()
//   {
//     base.OnStartClient();

    public void Start()
    {
        Main = this;
        C = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(isClient)
        {
            {
                Ray ray = C.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit Hit, MaxDistance, Mask))
                {
                    // print(Hit.transform.gameObject.name);

                    if (Hit.transform.GetComponent<Z_Interactee>())
                    {
                        if (hovering == false)
                        {
                            raycastHoverEnter.Raise();
                            // print("Hovering yes");
                            hovering = true;
                        }

                        if (Input.GetKeyDown(InteractKey) || Input.GetMouseButtonDown(0))      
                        {
                            Hit.transform.GetComponent<Z_Interactee>().Process();
                        } 

                        return;
                    }
                }

                if (hovering)
                {
                    raycastHoverExit.Raise();
                    // print("Hovring no");
                    hovering = false;
                }
            }
        }
    }
}
