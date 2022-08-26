using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum MOFATheTrainingPhase
{
    PlacingAvatar = 0,
    Fighting = 1
}

public class MOFATheTrainingRealityManager : RealityManager
{
    [SerializeField] private GameObject _placementIndicatorPrefab;

    [SerializeField] private float _enemyInitialDist;

    [SerializeField] private AvatarController _avatarPrefab;

    [Networked]
    public MOFATheTrainingPhase Phase { get; set; }

    private GameObject _placementIndicator;

    private ARRaycastManager _arRaycastManager;

    private AvatarController _avatar;

    private void Awake()
    {
        CoachingOverlaySessionDelegate.OnCoachingOverlayViewEnded += OnCoachingOverlayViewEnded;
        StARUIPanel.OnTriggerBtnPressed += OnTriggerBtnPressed;
    }

    private void OnDestroy()
    {
        CoachingOverlaySessionDelegate.OnCoachingOverlayViewEnded -= OnCoachingOverlayViewEnded;
        StARUIPanel.OnTriggerBtnPressed -= OnTriggerBtnPressed;
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public void SpawnPlacementIndicator()
    {
        if (_arRaycastManager == null)
        {
            _arRaycastManager = HoloKitCamera.Instance.gameObject.GetComponentInParent<ARRaycastManager>(true);
        }
        if (_placementIndicator == null)
        {
            _placementIndicator = Instantiate(_placementIndicatorPrefab);
            _arRaycastManager.enabled = true;
        }
    }

    public void DespawnPlacementIndicator()
    {
        if (_placementIndicator != null)
        {
            Destroy(_placementIndicator);
            _arRaycastManager.enabled = false;
        }
    }

    private void Update()
    {
        if (_placementIndicator != null)
        {
            // Raycast to find the floor
            Vector3 horizontalForward = Utils.GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);
            Vector3 rayOrigin = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 1.8f;
            Ray ray = new(rayOrigin, Vector3.down);
            List<ARRaycastHit> hitResults = new();
            if (_arRaycastManager.Raycast(ray, hitResults, TrackableType.Planes))
            {
                foreach (var hitResult in hitResults)
                {
                    var arPlane = hitResult.trackable.GetComponent<ARPlane>();
                    if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                    {
                        Vector3 p = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * _enemyInitialDist;
                        _placementIndicator.transform.position = new Vector3(p.x, hitResult.pose.position.y, p.z);
                        _placementIndicator.SetActive(true);
                        return;
                    }
                }
                _placementIndicator.SetActive(false);
            }
            else
            {
                _placementIndicator.SetActive(false);
            }
        }
    }

    private void OnCoachingOverlayViewEnded()
    {
        SpawnPlacementIndicator();
    }

    public void SpawnAvatar()
    {
        if (_placementIndicator.activeSelf)
        {
            _avatar = Runner.Spawn(_avatarPrefab, _placementIndicator.transform.position, _placementIndicator.transform.rotation);
            Destroy(_placementIndicator);
            Phase = MOFATheTrainingPhase.Fighting;
        }
        else
        {
            Debug.Log("Cannot spawn avatar in the current target position");
        }
    }

    private void OnTriggerBtnPressed()
    {
        if (Phase == MOFATheTrainingPhase.PlacingAvatar)
        {
            SpawnAvatar();
        }
        else
        {
            Debug.Log("Fire!");
        }
    }
}
