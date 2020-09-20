using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace NYUMultiplayerSpace
{
  public class ScreenSpacePickup : MonoBehaviour
  {

    [Serializable]
    public enum PickupState
    {
      Idle,
      attemptingPickup,
      Holding,
    }


    [Header("Input")]
    [SerializeField] private KeyCode pickupKey;

    [Header("Tuning")]
    [SerializeField] private FloatReference pickupRange;
    [SerializeField] private Vector3Reference pickupFloatingPlaneOffset;
    [SerializeField] private FloatReference positionSmoothingFactor;
    //[SerializeField] private Vector2Reference pickupFloatingPlaneDimensions;

    [Header("References")]
    [SerializeField] private GameObjectReference cameraReference;

    [ReadOnly]
    [SerializeField] private PickupState state = PickupState.Idle;

    [SerializeField] private LayerMask collisionLayerMask;


    //[SerializeField] private BoolReference attemptingPickup;

    private new Camera camera;
    private Vector3 raycastHitPosition;
    private Pickable objectPicked;

    private void OnValidate()
    {
      if (cameraReference == null)
        Debug.LogError("ScreenSpacePickup has no camera reference attached to it");
      if (cameraReference.Value.GetComponent<Camera>() == null)
        Debug.LogError("Screenspacepickup camera reference has no camera component");
    }

    private void Awake()
    {
      camera = cameraReference.Value.GetComponent<Camera>();
      state = PickupState.Idle;
    }

    private Vector3 objectTargetPosition
      => transform.position + (Vector3)(transform.localToWorldMatrix * pickupFloatingPlaneOffset.Value);

    // Update is called once per frame
    void Update()
    {
      if (state == PickupState.Idle)
      {
        if (Input.GetKeyDown(pickupKey))
        {
          state = PickupState.attemptingPickup;
        }
      }

      if (state == PickupState.attemptingPickup)
      {
        if (Input.GetKeyUp(pickupKey))
        {
          state = PickupState.Idle;
        }

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionLayerMask))
        {
          var pickable = hit.transform.GetComponent<Pickable>();
          raycastHitPosition = hit.transform.position;
          if (pickable != null)
          {
            if (Vector3.Distance(pickable.transform.position, transform.position) < pickupRange.Value)
            {
              objectPicked = pickable;
              objectPicked.Pick();
              state = PickupState.Holding;
            }
          }
        }
      }

      if (state == PickupState.Holding)
      {
        if (Input.GetKeyUp(pickupKey))
        {
          state = PickupState.Idle;
          objectPicked.Release();
        }
        else
        {
          objectPicked.transform.position = Vector3.Slerp(
            objectPicked.transform.position,
            objectTargetPosition,
            positionSmoothingFactor.Value);
        }
      }
    }

    private void OnDrawGizmos()
    {
      if (state == PickupState.attemptingPickup)
      {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pickupRange.Value);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(raycastHitPosition, 0.10f);
      }

      //var floatingPlane =
      //Gizmos.DrawLine();

      if (state == PickupState.Holding)
      {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(objectTargetPosition, 0.1f);
      }
    }
  }
}