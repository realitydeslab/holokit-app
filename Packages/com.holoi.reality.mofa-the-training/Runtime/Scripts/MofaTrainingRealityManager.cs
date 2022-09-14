using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Mofa.Base;
using Unity.Netcode;
using Holoi.HoloKit.App;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        public MofaAI MofaAIPrefab;

        public GameObject PlacementIndicatorPrefab;

        private GameObject _placementIndicator;

        private ARRaycastManager _arRaycastManager;

        private void Awake()
        {
            StARARPanel.OnTriggered += OnTriggered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            StARARPanel.OnTriggered -= OnTriggered;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // Spawn host's player
                SpawnMofaPlayer(MofaTeam.Blue, NetworkManager.LocalClientId);

                // Spawn host's life shield
                SpawnLifeShield(NetworkManager.LocalClientId);
            }

            if (IsServer)
            {
                _placementIndicator = Instantiate(PlacementIndicatorPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
                _arRaycastManager = HoloKitCamera.Instance.GetComponentInParent<ARRaycastManager>();
            }
        }

        private void Update()
        {
            if (_placementIndicator != null)
            {
                Vector3 horizontalForward = MofaUtils.GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);
                Vector3 rayOrigin = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 1.5f; // TODO: Changeable
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            Vector3 position = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 3f; // TODO: Changeable
                            _placementIndicator.transform.position = new Vector3(position.x, hit.pose.position.y, position.z);
                            _placementIndicator.SetActive(true);
                            return;
                        }
                    }
                }
                _placementIndicator.SetActive(false);
            }
        }

        private void SpawnMofaAI()
        {
            if (_placementIndicator.activeSelf)
            {
                var mofaAI = Instantiate(MofaAIPrefab, _placementIndicator.transform.position, Quaternion.identity);
                mofaAI.Team.Value = MofaTeam.Red;
                mofaAI.GetComponent<NetworkObject>().SpawnWithOwnership(999);

                Destroy(_placementIndicator);
                _arRaycastManager.enabled = false;
                HoloKitCamera.Instance.GetComponentInParent<ARPlaneManager>().enabled = false;
                DestroyExistingARPlanes();
            }
            else
            {
                Debug.Log("[MOFATheTraining] Failed to spawn AvatarController");
            }
        }

        private void OnTriggered()
        {
            if (Phase.Value == MofaPhase.Waiting)
            {
                SpawnMofaAI();
                StartCoroutine(StartSingleRound());
                return;
            }
        }

        private void DestroyExistingARPlanes()
        {
            var planes = FindObjectsOfType<ARPlane>();
            foreach (var plane in planes)
            {
                Destroy(plane.gameObject);
            }
        }
    }
}
