using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutCommon
{
  [RequireComponent(typeof(Camera))]
  public class CameraMethods : MonoBehaviour
  {
    private new Camera camera;
    private LayerMask initialMask;

    private void Awake()
    {
      camera = GetComponent<Camera>();
      initialMask = camera.cullingMask;
    }

    public void ResetCullingMask()
    {
      camera.cullingMask = initialMask;
    }

    public void SetCullingMask(string layerName)
    {
      // nothing to see here! - cosmo d

      // var mask = LayerMask.NameToLayer(layerName);
      // print(layerName);
      // print(mask);
      // camera.cullingMask = 1 << mask;
    }
  }
}