﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomizerData : MessageBase
{
    public string userName;
    public bool bIsModerator;
    public int[] bodyIDs = new int[4];

    // make sure this array lenght matches torsoNodes & torsoNodeScales
    // from the inspector
    public float[] torsoScales = new float[8];
    public Vector3[] bodyColors = new Vector3[5];
}
