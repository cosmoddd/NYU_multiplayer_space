using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using System.Linq;
using Mirror;

public class PlayerRespawner : LocalPlayerOnlyNetworkBehaviour
{
    [SerializeField] private GameObjectValueList spawnPoints;

    public void MoveToRandomSpawnPoint(CharacterController characterController)
    {
        var spawnPoint = spawnPoints.ElementAt(Random.Range(0, spawnPoints.Count));
        characterController.enabled = false;
        characterController.transform.position = spawnPoint.transform.position;
        characterController.enabled = true;
    }
}
