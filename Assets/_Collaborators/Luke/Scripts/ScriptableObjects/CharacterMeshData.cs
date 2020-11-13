using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterMeshData : ScriptableObject
{
    // must have Serializable attribute to allow array
    // to appear in inspector
    //[Serializable]
    //public struct PresetArray
    //{
    //    public float[] presetValues;
    //}

    [Serializable]
    public struct MeshesArray
    {
        public string name;
        public Mesh[] meshes;
    }

    //public PresetArray[] bodyPresets;
    public MeshesArray[] bodyMeshes;
}
