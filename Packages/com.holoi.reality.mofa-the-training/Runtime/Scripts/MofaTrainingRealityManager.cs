using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Mofa.Base;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
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

        private MofaAI _mofaAI;

        protected override void Awake()
        {
            base.Awake();

            HoloKitAppUIEventsReactor.OnTriggered += OnTriggered;
            HoloKitAppUIEventsReactor.OnBoosted += OnBoosted;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            HoloKitAppUIEventsReactor.OnTriggered -= OnTriggered;
            HoloKitAppUIEventsReactor.OnBoosted -= OnBoosted;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                SpawnLocalPlayerSpellManager();
                // Spawn host's player
                SpawnMofaPlayer(MofaTeam.Blue, NetworkManager.LocalClientId);
                // Spawn host's life shield
                SpawnLifeShield(NetworkManager.LocalClientId);

                SpawnMofaAI();
            }

            if (IsServer)
            {
                var placementIndicatorInitialPos = HoloKitHelper.IsRuntime ? Vector3.zero : new Vector3(0f, -1f, 5f);
                _placementIndicator = Instantiate(PlacementIndicatorPrefab, placementIndicatorInitialPos, Quaternion.identity);
                _arRaycastManager = HoloKitCamera.Instance.GetComponentInParent<ARRaycastManager>();
            }
        }

        private void Update()
        {
            if (_placementIndicator != null)
            {
                if (HoloKitHelper.IsRuntime)
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
        }

        private void SpawnMofaAI()
        {
            _mofaAI = Instantiate(MofaAIPrefab);
            _mofaAI.Team.Value = MofaTeam.Red;
            _mofaAI.GetComponent<NetworkObject>().SpawnWithOwnership(999);
        }

        private void OnTriggered()
        {
            if (Phase.Value == MofaPhase.Waiting)
            {
                if (_placementIndicator.activeSelf)
                {
                    _mofaAI.InitialPosition.Value = _placementIndicator.transform.position;
                    Destroy(_placementIndicator);
                    StartCoroutine(StartSingleRound());
                }
                else
                {
                    Debug.Log("[MOFATheTraining] Cannot start game in current position");
                }
                return;
            }

            if (Phase.Value == MofaPhase.Countdown)
            {
                var arRaycastManager = HoloKitCamera.Instance.GetComponentInParent<ARRaycastManager>();
                if (arRaycastManager != null)
                {
                    arRaycastManager.enabled = false;
                }
                var arPlaneManager = HoloKitCamera.Instance.GetComponentInParent<ARPlaneManager>();
                if (arPlaneManager != null)
                {
                    arPlaneManager.enabled = false;
                    DestroyExistingARPlanes();
                }
                return;
            }

            if (Phase.Value == MofaPhase.Fighting)
            {
                LocalPlayerSpellManager.SpawnBasicSpell();
                return;
            }

            if (Phase.Value == MofaPhase.RoundData)
            {
                StartCoroutine(StartSingleRound());
                return;
            }
        }

        private void OnBoosted()
        {
            if (Phase.Value == MofaPhase.Fighting)
            {
                LocalPlayerSpellManager.SpawnSecondarySpell();
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
