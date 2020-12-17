using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToGame : MonoBehaviour
{
  public string thisURL;

    public void OpenUrl()
    {
      Application.OpenURL(thisURL);
    }
}
