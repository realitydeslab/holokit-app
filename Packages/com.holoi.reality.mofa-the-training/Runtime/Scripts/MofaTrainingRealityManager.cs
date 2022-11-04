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

        private MofaPlayerAI _mofaPlayerAI;

        private AvatarPlacementIndicatorController _placementIndicator;

        protected override void Start()
        {
            base.Start();

            StartCoroutine(HoloKitAppUtils.WaitAndDo(1.5f, () =>
            {
                if (HoloKitApp.Instance.IsHost)
                {
                    HoloKitApp.Instance.ARSessionManager.AddARPlaneManager();
                    HoloKitApp.Instance.ARSessionManager.AddARRaycastManager();
                }
            }));
            OnPhaseChanged += OnPhaseChangedFunc;
            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived += OnStartRoundMessageReceived;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                SpawnMofaPlayerAI();
                var placementIndicatorInitialPos = HoloKitUtils.IsRuntime ? Vector3.zero : new Vector3(0f, -1f, 5f);
                _placementIndicator = Instantiate(_placementIndicatorPrefab, placementIndicatorInitialPos, Quaternion.identity);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            OnPhaseChanged -= OnPhaseChangedFunc;
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
            if (_placementIndicator != null
                && HoloKitApp.Instance.ARSessionManager.ARRaycastManager != null
                && HoloKitApp.Instance.ARSessionManager.ARPlaneManager != null)
            {
                if (HoloKitUtils.IsRuntime)
                {
                    Vector3 horizontalForward = MofaUtils.GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);
                    Vector3 rayOrigin = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 1.5f; // TODO: Changeable
                    Ray ray = new(rayOrigin, Vector3.down);
                    List<ARRaycastHit> hits = new();
                    if (HoloKitApp.Instance.ARSessionManager.ARRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                    {
                        foreach (var hit in hits)
                        {
                            var arPlane = hit.trackable.GetComponent<ARPlane>();
                            if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                            {
                                Vector3 position = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 6f; // TODO: Changeable
                                _placementIndicator.transform.position = new Vector3(position.x, hit.pose.position.y, position.z);
                                Vector3 forwardVector = HoloKitCamera.Instance.CenterEyePose.position - _placementIndicator.transform.position;
                                _placementIndicator.transform.rotation = MofaUtils.GetHorizontalLookRotation(forwardVector);
                                _placementIndicator.gameObject.SetActive(true);
                                return;
                            }
                        }
                    }
                    _placementIndicator.gameObject.SetActive(false);
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
                    _mofaPlayerAI.InitializeAvatarPositionClientRpc(_placementIndicator.transform.position, _placementIndicator.transform.rotation);
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

        private void OnPhaseChangedFunc(MofaPhase mofaPhase)
        {
            if (CurrentPhase == MofaPhase.Countdown)
            {
                HoloKitApp.Instance.ARSessionManager.SetARPlaneManagerActive(false);
                HoloKitApp.Instance.ARSessionManager.SetARRaycastManagerActive(false);
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
