using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;
using Holoi.Library.MOFABase.WatchConnectivity;
using HoloKit;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private MofaPlayerAI _mofaPlayerAIPrefab;

        [SerializeField] private AvatarPlacementIndicatorController _placementIndicatorPrefab;

        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        private MofaPlayerAI _mofaPlayerAI;

        private AvatarPlacementIndicatorController _placementIndicator;

        private const float RaycastHorizontalOffset = 1.2f;

        private const float AvatarSpawnHorizontalOffset = 6f;

        public static event Action OnFoundPlane;

        public static event Action OnLostPlane;

        protected override void Start()
        {
            base.Start();

            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived += OnStartRoundMessageReceived;

            if (HoloKitApp.Instance.IsHost)
            {
                var placementIndicatorInitialPos = HoloKitUtils.IsRuntime ? Vector3.zero : new Vector3(0f, -1f, 5f);
                _placementIndicator = Instantiate(_placementIndicatorPrefab, placementIndicatorInitialPos, Quaternion.identity);
                if (HoloKitUtils.IsRuntime)
                {
                    _placementIndicator.gameObject.SetActive(false);
                }
                else
                {
                    _placementIndicator.gameObject.SetActive(true);
                    OnFoundPlane?.Invoke();
                }

                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                SpawnMofaPlayerAI();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived -= OnStartRoundMessageReceived;
        }

        private void SpawnMofaPlayerAI()
        {
            _mofaPlayerAI = Instantiate(_mofaPlayerAIPrefab);
            // TODO: This is hard-coded
            _mofaPlayerAI.MagicSchoolTokenId.Value = 0;
            _mofaPlayerAI.Team.Value = MofaTeam.Red;
            _mofaPlayerAI.GetComponent<NetworkObject>().SpawnWithOwnership(999);
        }

        private void Update()
        {
            if (HoloKitUtils.IsEditor) { return; }

            if (_placementIndicator != null && _arPlaneManager.enabled)
            {
                Vector3 horizontalForward = MofaUtils.GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);
                Vector3 rayOrigin = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * RaycastHorizontalOffset;
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            Vector3 position = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * AvatarSpawnHorizontalOffset;
                            _placementIndicator.transform.position = new Vector3(position.x, hit.pose.position.y, position.z);
                            Vector3 forwardVector = HoloKitCamera.Instance.CenterEyePose.position - _placementIndicator.transform.position;
                            _placementIndicator.transform.rotation = MofaUtils.GetHorizontalLookRotation(forwardVector);
                            if (!_placementIndicator.gameObject.activeSelf)
                            {
                                _placementIndicator.gameObject.SetActive(true);
                                OnFoundPlane?.Invoke();
                            }
                            return;
                        }
                    }
                }
                if (_placementIndicator.gameObject.activeSelf)
                {
                    _placementIndicator.gameObject.SetActive(false);
                    OnLostPlane?.Invoke();
                }
            }
        }

        protected override void StartRound()
        {
            if (CurrentPhase != MofaPhase.Waiting && CurrentPhase != MofaPhase.RoundData)
            {
                Debug.Log($"[MofaBaseRealityManager] You cannot start round at the current phase: {CurrentPhase}");
                return;
            }

            if (RoundCount == 0)
            {
                if (_placementIndicator.gameObject.activeSelf)
                {
                    _placementIndicator.OnBirth();
                    _mofaPlayerAI.InitializeAvatarClientRpc(_placementIndicator.transform.position,
                                                            _placementIndicator.transform.rotation,
                                                            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId].MetaAvatarCollectionBundleId,
                                                            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId].MetaAvatarTokenId);
                    _arPlaneManager.enabled = false;
                    _arRaycastManager.enabled = false;
                    Destroy(_placementIndicator.gameObject, 3f);
                    StartCoroutine(StartRoundFlow());
                }
                else
                {
                    Debug.Log("[MOFATheTraining] Cannot start round in current position");
                }
            }
            else
            {
                StartCoroutine(StartRoundFlow());
            }
        }

        private void OnTriggered()
        {
            if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
            {
                StartRound();
            }
        }

        // Apple Watch
        private void OnStartRoundMessageReceived()
        {
            StartRound();
        }
    }
}
