using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSecret : MonoBehaviour
{
  public GameObject hiddenThing;
  public string password;
  public void UpdateString(string value)
  {
    hiddenThing.SetActive(value == password);
  }
}
