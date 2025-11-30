using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Project.Runtime
{
    public class ImageTracker : MonoBehaviour
    {
        private ARTrackedImageManager trackedImages;
        public GameObject[] ArPrefabs;
    
        List<GameObject> ARObjects = new List<GameObject>();
    
        private void Awake()
        {
            trackedImages = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            trackedImages.trackedImagesChanged += OnImageChanged;
        }
        
        private void OnDisable()
        {
            trackedImages.trackedImagesChanged -= OnImageChanged;
        }
        
        private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var trackedImage in args.added)
            {
                foreach (var prefab in ArPrefabs)
                {
                    if (trackedImage.referenceImage.name == prefab.name)
                    {
                        var newPrefab = Instantiate(prefab, trackedImage.transform);
                        ARObjects.Add(newPrefab);
                    }
                }
            }
            
            foreach (var trackedImage in args.updated)
            {
                foreach (var go in ARObjects)
                {
                    if (trackedImage.referenceImage.name == go.name)
                    {
                        go.SetActive(trackedImage.trackingState == TrackingState.Tracking);
                    }
                }
            }
        }
    }
}
