using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPriorit : MonoBehaviour
{
    // set the thread to slam through loading the level.  Don't hold back!
    void Awake()
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
    }


}
