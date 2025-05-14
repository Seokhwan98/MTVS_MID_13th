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
        Debug.Log(trackedImage.name);
        
        string referenceImageName = trackedImage.referenceImage.name;
        
        spawnedObjects[referenceImageName].transform.position = trackedImage.transform.position;
        spawnedObjects[referenceImageName].transform.rotation = trackedImage.transform.rotation;
        
        spawnedObjects[referenceImageName].SetActive(true);
    }

    private void Update()
    {
       //확인용 디버그 로그
       Debug.Log($"Image: {trackedImageManager.trackables.count} tracked");
       foreach (var trackedImage in trackedImageManager.trackables)
       {
           Debug.Log($"Image : {trackedImage.referenceImage.name}\n" +
                     $"Pos : {trackedImage.transform.position}");
       }
       
    }
}
