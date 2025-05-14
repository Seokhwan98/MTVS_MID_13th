using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

    void OntrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateSpawnObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateSpawnObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;
        
        spawnedObjects[referenceImageName].transform.position = trackedImage.transform.position;
        spawnedObjects[referenceImageName].transform.rotation = trackedImage.transform.rotation;
        
        spawnedObjects[referenceImageName].SetActive(true);
    }

    private void Update()
    {
       //확인용 디버그 로그
       Debug.Log($"Image: {trackedImageManager.trackables.count} 이미지가 트래킹 되고있슴댜");
       foreach (var trackedImage in trackedImageManager.trackables)
       {
           Debug.Log($"Image : {trackedImage.referenceImage.name}이름의 이미지며" +
                     $"{trackedImage.transform.position}의 위치에 있습니다");
       }
       
    }
}
