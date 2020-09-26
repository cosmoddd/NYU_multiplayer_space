using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NYUMultiplayerSpace
{
  public class Pickable : MonoBehaviour
  {
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void Pick()
    {
      var rigidbody = GetComponent<Rigidbody>();
      if (rigidbody != null)
      {
        // JOINT?
        rigidbody.isKinematic = true;
      }
    }

    internal void Release()
    {
      var rigidbody = GetComponent<Rigidbody>();
      if (rigidbody != null)
      {
        rigidbody.isKinematic = false;
      }
    }
  }
}