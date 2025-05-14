using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectOnPlane : MonoBehaviour
{
    public UnityEvent OnComplete;
    public GameObject spawnPrefab;
    
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
                OnComplete?.Invoke();
            }
        }
    }

    public void ResetAR()
    {
        // 배치된 오브젝트 제거
        try
        {
            var anchorObjects = GameObject.FindGameObjectsWithTag("PlacedObject");

            if (anchorObjects.Length == 0)
            {
                Debug.Log("[AR] 제거할 오브젝트가 없습니다.");
                return;
            }

            foreach (var obj in anchorObjects)
            {
                Destroy(obj);
            }

            Debug.Log("[AR] 모든 배치된 오브젝트 제거 완료");
        }
        catch (UnityException e)
        {
            Debug.LogWarning($"[AR] 태그 'PlacedObject'가 존재하지 않습니다. 태그를 등록해주세요.\n{e.Message}");
        }

        // AnchorManager는 자동으로 관리되므로 따로 제거하지 않아도 됨 (필요시 직접 추적)

        // Plane 감지 다시 활성화
        planeManager.enabled = true;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

        // 상태 초기화
        hasSpawned = false;
    }
}
