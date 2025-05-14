using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    
    private ARRaycastManager raycastManager;
    private ARAnchorManager anchorManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new();
    private static bool hasSpawned = false;
    
    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (Touchscreen.current == null || hasSpawned)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                TrackableId planeId = hits[0].trackableId;
                ARPlane plane = GetComponent<ARPlaneManager>().GetPlane(planeId);
                
                ARAnchor anchor = anchorManager.AttachAnchor(plane, hitPose);
                if (anchor == null)
                {
                    Debug.Log("Anchor 생성 실패");
                    return;
                }
                
                Instantiate(spawnPrefab, anchor.transform);
                hasSpawned = true;
                
                foreach (var item in planeManager.trackables)
                {
                    item.gameObject.SetActive(false);
                }
                planeManager.enabled = false;
            }
        }
    }
}
