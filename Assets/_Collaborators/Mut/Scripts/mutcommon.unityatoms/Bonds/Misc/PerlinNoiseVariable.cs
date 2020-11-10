using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace MutCommon.UnityAtoms
{
  public class PerlinNoiseVariable : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] private FloatVariable output;
    [Header("Parameters")]
    [SerializeField] private FloatReference speed;
    [SerializeField] private FloatReference seed;
    [SerializeField] private IntReference octaves;
    [SerializeField] private FloatReference persistence;
#pragma warning restore 0649

    private float pos;

    public void Update()
    {
      pos += speed * Time.deltaTime;
      output.Value = MutCommon.NoiseUtils.OctavePerlin(pos, seed.Value, octaves.Value, persistence.Value);
    }
  }
}