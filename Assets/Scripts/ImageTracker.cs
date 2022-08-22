using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ImageTracker : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;

    [SerializeField] private Image _imageSlot;

    [SerializeField] private Sprite _trackedImage;

    private int _scannedCount = 0;

    private void Start()
    {
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        App.OnSpectatorJoined += OnSpectatorJoined;
    }

    private void OnDestroy()
    {
        App.OnSpectatorJoined -= OnSpectatorJoined;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.updated)
        {
            Debug.Log("Image added");
            if (_scannedCount == 0)
            {
                _scannedCount++;

                float theta = image.transform.rotation.eulerAngles.y;
                Vector3 position = image.transform.position +
                    Quaternion.Euler(new Vector3(0f, -App.Instance.RealityManager.HoriontalRotation, 0f))
                    * Quaternion.Euler(0f, theta, 0f) * -App.Instance.RealityManager.PosePosition;
                float horizontalRotation = theta - App.Instance.RealityManager.HoriontalRotation;
                //App.Instance.RealityManager.ResetOrigin(position, Quaternion.Euler(0f, horizontalRotation, 0f));
            }
        }

        foreach (var image in args.updated)
        {
            Debug.Log($"Image updated {image.transform.position} {image.transform.rotation}");
        }

        foreach (var image in args.removed)
        {
            Debug.Log($"Image removed");
        }
    }

    private void OnSpectatorJoined()
    {
        _imageSlot.sprite = _trackedImage;
    }

    public void StartFindingImage()
    {
        _arTrackedImageManager.enabled = true;
    }

    public void StopFindingImage()
    {
        _arTrackedImageManager.enabled = false;
    }
}
