using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class StartingPosition : MonoBehaviour
{
    [SerializeField] private GameObjectValueList spawnPoints;

    void Awake()
    {
        spawnPoints.Add(this.gameObject);
    }
}
