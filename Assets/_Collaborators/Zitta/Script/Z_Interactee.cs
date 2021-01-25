using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityAtoms.BaseAtoms;

public class Z_Interactee : NetworkBehaviour {

    public UnityEvent onPressed;
    public List<MeshRenderer> Meshes;
    [HideInInspector] public bool LastActive;
    Outline thisOutlineScript;

  public override void OnStartServer()
  {
    base.OnStartServer();
    if (isServerOnly)
    {
      print("Removing Outliner script " + this.gameObject.name);
      {
        thisOutlineScript = GetComponent<Outline>();
        if (thisOutlineScript) Destroy(thisOutlineScript);
      }
    }
  }



    void Start()
    {
      // thisOutlineScript = GetComponent<Outline>();
    }

    public void Process()
    {
        // SendMessage("Interact");
        // print("Interact");

        onPressed.Invoke();
    }

    public void Update()
    {
        if (!Z_Interactor.Main)
            return;

        if (this == Z_Interactor.Main.HoveredObject && !LastActive)
        {
            if (Z_Interactor.Main.StartOutline(this))
                LastActive = true;
        }
        else if (this != Z_Interactor.Main.HoveredObject && LastActive)
        {
            if (Z_Interactor.Main.EndOutline(this))
                LastActive = false;
        }
    }
}
