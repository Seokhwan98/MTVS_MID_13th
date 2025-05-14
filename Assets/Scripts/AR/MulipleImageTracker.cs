using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MulipleImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects;
    
    private bool centerInitialized = false;
    private Vector3 tableCenter;
    [SerializeField] private float centerOffsetDistance = 1.0f;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        spawnedObjects = new Dictionary<string, GameObject>();

        foreach (GameObject obj in placeablePrefabs)
        {
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            newObject.SetActive(false);
            
            spawnedObjects.Add(newObject.name, newObject);
        }
    }

    public void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (!centerInitialized)
            {
                // 최초 인식된 마커 기준으로 테이블 중심 계산
                tableCenter = trackedImage.transform.position + trackedImage.transform.forward * centerOffsetDistance;
                centerInitialized = true;
                Debug.Log($"[reset]center : {tableCenter}");
            }
            UpdateSpawnObject(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.None)
            {
                // 이미지가 추적되지 않음
                if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject spawnedObject))
                {
                    spawnedObject.SetActive(false);
                }
            }
            else
            {
                // 이미지가 추적 중이거나 제한적으로 추적 중
                UpdateSpawnObject(trackedImage);
            }
        }
    }

    private void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;
        GameObject obj = spawnedObjects[referenceImageName];

        obj.transform.position = tableCenter;

        // 콘텐츠는 마커를 바라보게 회전
        Vector3 lookDirection = trackedImage.transform.position - tableCenter;
        lookDirection.y = 0f; // 수평 회전만

        if (lookDirection != Vector3.zero)
            obj.transform.rotation = Quaternion.LookRotation(lookDirection);

        obj.SetActive(true);
        
        SeaControl sc = obj.GetComponentInChildren<SeaControl>();
        if (sc != null)
        {
            sc.InitializeBounds(); // 바운드를 옮긴 위치로 갱신
        }
    }
    
    public void ResetTableCenter()
    {
        Debug.Log("[reset]");

        centerInitialized = false;

        // 모든 오브젝트 비활성화
        foreach (var obj in spawnedObjects.Values)
        {
            obj.SetActive(false);
        }
    }

    private void Update()
    {
       //확인용 디버그 로그
       // Debug.Log($"Image: {trackedImageManager.trackables.count} tracked");
       foreach (var trackedImage in trackedImageManager.trackables)
       {
           Debug.Log($"Image : {trackedImage.referenceImage.name}\n" +
                     $"Pos : {trackedImage.transform.position}");
       }
       
    }
}