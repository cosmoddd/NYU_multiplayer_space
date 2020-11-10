using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace MutCommon.UnityAtoms
{
  public class LookAtObject : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] private GameObjectReference target;
#pragma warning restore 0649

    // Update is called once per frame
    void Update()
    {
      if (target?.Value == null) return;
      transform.LookAt(target.Value.transform);
    }
  }
}
