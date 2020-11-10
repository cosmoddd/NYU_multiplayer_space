using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace MutCommon.UnityAtoms
{
  public class RotateObject : MonoBehaviour
  {

#pragma warning disable 0649
    [SerializeField]
    private Vector3Reference Axis;

    [SerializeField]
    private FloatReference RotationsPerSecond;
#pragma warning restore 0649

    // Update is called once per frame
    void Update()
    {
      transform.Rotate(Axis, Time.deltaTime * 360 * RotationsPerSecond);
    }
  }
}