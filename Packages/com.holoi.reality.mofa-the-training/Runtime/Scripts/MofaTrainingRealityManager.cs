using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Training")]
        public MofaAI MofaAIPrefab;

        public GameObject PlacementIndicatorPrefab;

        private GameObject _placementIndicator;

        private ARRaycastManager _arRaycastManager;

        private MofaAI _mofaAI;

        protected override void Awake()
        {
            base.Awake();

            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChangedFunc;
            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
            HoloKitAppUIEventManager.OnBoosted += OnBoosted;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChangedFunc;
            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
            HoloKitAppUIEventManager.OnBoosted -= OnBoosted;
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
                                Vector3 position = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * 5; // TODO: Changeable
                                _placementIndicator.transform.position = new Vector3(position.x, hit.pose.position.y, position.z);
                                Vector3 forwardVector = HoloKitCamera.Instance.CenterEyePose.position - _placementIndicator.transform.position;
                                _placementIndicator.transform.rotation = MofaUtils.GetHorizontalLookRotation(forwardVector);
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
                    _mofaAI.InitializeAvatarPositionClientRpc(_placementIndicator.transform.position, _placementIndicator.transform.rotation);
                    Destroy(_placementIndicator);
                    StartCoroutine(StartSingleRound());
                }
                else
                {
                    Debug.Log("[MOFATheTraining] Cannot start game in current position");
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

        private void OnPhaseChangedFunc(MofaPhase mofaPhase)
        {
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
