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
    public LayerMask RangeMask;
    public float RangeDistance;

    public VoidEvent raycastHoverEnter;
    public VoidEvent raycastHoverExit;
    
    public bool hovering = false;
    public Z_Interactee HoveredObject;

    public Z_Interactee OutlinedObject;
    public Material OutlineMaterial;
    public List<Material> SavedMaterials;

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
                Ray rayII = new Ray(C.transform.position, C.transform.forward);
                
                Z_Interactee interactee = null;
                bool MouseInput = false;

                if (Physics.Raycast(ray, out RaycastHit Hit, MaxDistance, Mask))
                {
                    MouseInput = true;

                    if (Hit.collider.transform.GetComponent<Z_Interactee>())
                        interactee = Hit.collider.transform.GetComponent<Z_Interactee>();
                    else if (Hit.transform.GetComponent<Z_Interactee>())
                        interactee = Hit.transform.GetComponent<Z_Interactee>();
                    else
                        MouseInput = false;
                }
                else if (Physics.Raycast(rayII, out RaycastHit HitII, RangeDistance, RangeMask))
                {
                    if (HitII.collider.transform.GetComponent<Zitta_InteractionRange>())
                    {
                        Camera C = Zitta_CameraDetection.Main.C;
                        Collider C2D = HitII.collider.transform.GetComponent<Zitta_InteractionRange>().RealCollider;
                        if (!C2D || GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(C), C2D.bounds))
                            interactee = HitII.collider.transform.GetComponent<Zitta_InteractionRange>().Target;
                    }
                }

                if (interactee)
                {
                    if (HoveredObject && HoveredObject != interactee)
                    {
                      if (HoveredObject.GetComponent<Outline>()) HoveredObject.GetComponent<Outline>().enabled=false;
                    }

                    if (HoveredObject && HoveredObject == interactee)
                    {
                      if (HoveredObject.GetComponent<Outline>()) HoveredObject.GetComponent<Outline>().enabled=true;
                    }

                    if (Input.GetKeyDown(InteractKey) || Input.GetMouseButtonDown(0))
                        interactee.Process();
                    HoveredObject = interactee;


                }
                else
                {
                    if (HoveredObject && HoveredObject.GetComponent<Outline>())
                    {
                      HoveredObject.GetComponent<Outline>().enabled = false;    
                    }
                    HoveredObject = null;
                }

                if (interactee && MouseInput)
                {
                    if (hovering == false)
                    {
                        raycastHoverEnter.Raise();

                        if (HoveredObject.GetComponent<Outline>())
                        {
                          HoveredObject.GetComponent<Outline>().enabled = true;    
                        }


                        hovering = true;
                    }
                }
                else
                {
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

    //  no longer need this  --v

    public bool StartOutline(Z_Interactee Target)
    {
        if (OutlinedObject)
            return false;
        OutlinedObject = Target;
        SavedMaterials = new List<Material>();
        for (int I = 0; I < Target.Meshes.Count; I++)
        {
            for (int i = 0; i < Target.Meshes[I].materials.Length; i++)
            {
                SavedMaterials.Add(Target.Meshes[I].materials[i]);
                Target.Meshes[I].materials[i] = OutlineMaterial;
            }
        }
        return true;
    }

    public bool EndOutline(Z_Interactee Target)
    {
        if (OutlinedObject != Target)
            return false;
        OutlinedObject = null;
        int Index = 0;
        for (int I = 0; I < Target.Meshes.Count; I++)
        {
            for (int i = 0; i < Target.Meshes[I].materials.Length; i++)
            {
                Target.Meshes[I].materials[i] = SavedMaterials[Index];
                Index++;
            }
        }
        SavedMaterials = new List<Material>();
        return true;
    }
}
