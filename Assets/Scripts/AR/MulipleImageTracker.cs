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
    private Dictionary<string, Vector3> tableCenters = new();
    private HashSet<string> initialized = new HashSet<string>();
    
    private bool centerInitialized = false;
    private Vector3 tableCenter;
    [SerializeField] private float centerOffsetDistance = 1.0f;
    [SerializeField] private float disableDistanceThreshold = 5f;


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
            var name = trackedImage.referenceImage.name;
            if (initialized.Contains(name)) continue;      // 이미 초기화했던 마커면 스킵
            initialized.Add(name);

            var obj = spawnedObjects[name];
            obj.transform.SetParent(trackedImage.transform, false);
            obj.transform.localPosition = new Vector3(0, 0, centerOffsetDistance);
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
            
            // ✅ 오브젝트 내부의 SeaControl이 있다면 바운드 재계산
            SeaControl sc = obj.GetComponentInChildren<SeaControl>();
            if (sc != null)
            {
                sc.InitializeBounds(); // 바운드를 옮긴 위치로 갱신
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            var name = trackedImage.referenceImage.name;
            if (!spawnedObjects.TryGetValue(name, out var obj)) continue;

            // 월드 상에서 마커 위치 ↔ 객체 위치 거리
            float dist = Vector3.Distance(trackedImage.transform.position, obj.transform.position);
            obj.SetActive(dist <= disableDistanceThreshold);
        }
    }
    
    private void LateUpdate()
    {
        string closestName = null;
        float closestDist = float.MaxValue;

        // 1) 각 트래킹된 이미지 ⇆ 그 자식 오브젝트 거리 계산
        foreach (var trackedImage in trackedImageManager.trackables)
        {
            if (!spawnedObjects.ContainsKey(trackedImage.referenceImage.name)) continue;

            var obj = spawnedObjects[trackedImage.referenceImage.name];
            float d = Vector3.Distance(trackedImage.transform.position, obj.transform.position);
            if (d < closestDist && d <= disableDistanceThreshold)
            {
                closestDist = d;
                closestName = trackedImage.referenceImage.name;
            }
        }

        // 2) 가장 가까운 것만 활성화 (나머지 비활성화)
        foreach (var kv in spawnedObjects)
            kv.Value.SetActive(kv.Key == closestName);
    }
    // private void Update()
    // {
    //    //확인용 디버그 로그
    //    // Debug.Log($"Image: {trackedImageManager.trackables.count} tracked");
    //    foreach (var trackedImage in trackedImageManager.trackables)
    //    {
    //        Debug.Log($"Image : {trackedImage.referenceImage.name}\n" +
    //                  $"Pos : {trackedImage.transform.position}");
    //    }
    //    
    // }
}