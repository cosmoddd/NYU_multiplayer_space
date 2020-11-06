using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncListExample : MonoBehaviour
{
    // Create a struct to store the variables you need
    // The struct can be named anything you want
    public struct TraitData
    {
        // Here I'm storing a Vector3 and an int in this struct
        public Vector3 color;
        public int bodyID;

        // I'm also creating a constructor here so that we can
        // initialize the values when creating a TraitData object
        public TraitData(Vector3 inColor, int ID)
        {
            color = inColor;
            bodyID = ID;
        }
    };

    // Here we create a class that inherits from the SyncList class
    // using the struct that we just created 
    // ex. SyncList<struct>
    public class SyncTrait : SyncList<TraitData> {}

    // Now we can use SyncTrait exactly how you would use List<>
    public SyncTrait bodyTraits = new SyncTrait();

    void Start()
    {
        // When adding objects to the SyncList
        // ensure that you are creating objects from the struct you used
        // to create the custom list class
        TraitData exampleObject = new TraitData(Vector3.zero, 0);
        bodyTraits.Add(exampleObject);
    }
}
