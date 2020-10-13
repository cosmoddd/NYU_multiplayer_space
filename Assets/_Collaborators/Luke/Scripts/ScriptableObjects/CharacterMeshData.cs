using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterMeshData : ScriptableObject
{
    [Serializable]
    public class PresetArray
    {
        public float[] presetValues;
    }

    public Mesh[] hatMeshes;
    public Mesh[] headMeshes;
    public Mesh[] rightFootMeshes;
    public Mesh[] leftFootMeshes;
    public PresetArray[] bodyPresets;   
}
