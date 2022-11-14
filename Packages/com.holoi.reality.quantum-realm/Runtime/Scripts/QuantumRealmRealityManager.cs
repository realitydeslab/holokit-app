using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.QuantumRealm
{
    public class QuantumRealmRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementIndicator _arPlacementIndicator;

        [SerializeField] private HoverableStartButton _hoverableStartButton;

        [Header("Hand")]
        [SerializeField] private GameObject _extendedHandJoint;

        public HoverObject HostHandPose; // This is networked

        [SerializeField] private GameObject _hostHandVisual;

        [Header("Apple")]
        public CoreHapticsManager CoreHapticsManager;

        [Header("Quantum Realm")]
        [SerializeField] private NetworkObject _buddhaGroupPrefab;

        private BuddhaGroup _buddhaGroup;

        private readonly NetworkVariable<bool> _isHostHandValid = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
                HoloKitHandTracker.Instance.IsActive = true;
                _hostHandVisual.SetActive(false);
                _arPlacementIndicator.IsActive = true;

                // For debug
                if (HoloKitUtils.IsEditor)
                {
                    _hostHandVisual.SetActive(true);
                }
            }
            else
            {
                // Delete unnecessary objects on client at the very beginning
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_hoverableStartButton.gameObject);
                Destroy(_extendedHandJoint);
                // The networked hand is took control by the host
                HostHandPose.GetComponent<FollowTargetController>().MovementType = MovementType.None;
            }
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isHostHandValid.OnValueChanged += OnIsHostHandValidValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isHostHandValid.OnValueChanged -= OnIsHostHandValidValueChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.OnHandValidityChanged -= OnHandValidityChanged;
            }
        }

        public void OnSessionStarted()
        {
            // Spawn the buddha group
            SpawnBuddhaGroup();

            // Turn off indicators and buttons
            _arPlacementIndicator.OnPlacedFunc();
            _hoverableStartButton.OnDeath();

            // Turn off plane detection and raycast
            HoloKitApp.Instance.ARSessionManager.SetARPlaneManagerEnabled(false);
            HoloKitApp.Instance.ARSessionManager.SetARRaycastManagerEnabled(false);
        }

        private void SpawnBuddhaGroup()
        {
            var hitPoint = _arPlacementIndicator.HitPoint;
            Vector3 position = new(HoloKitCamera.Instance.CenterEyePose.transform.position.x,
                                   hitPoint.position.y,
                                   HoloKitCamera.Instance.CenterEyePose.transform.position.z);
            var buddhaGroup = Instantiate(_buddhaGroupPrefab, position, hitPoint.rotation * Quaternion.Euler(180f * Vector3.up));
            buddhaGroup.Spawn();
        }

        public void SetBuddhaGroup(BuddhaGroup buddhaGroup)
        {
            _buddhaGroup = buddhaGroup;
        }

        private void OnHandValidityChanged(bool isValid)
        {
            if (IsServer)
            {
                _isHostHandValid.Value = isValid;
            }
        }

        private void OnIsHostHandValidValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                if (IsServer)
                {
                    HostHandPose.transform.position = HoloKitCamera.Instance.CenterEyePose.position + 0.5f * Vector3.down;
                }
                HostHandPose.IsActive = true;
                _hostHandVisual.SetActive(true);
                return;
            }

            if (oldValue && !newValue)
            {
                HostHandPose.IsActive = false;
                _hostHandVisual.SetActive(false);
                return;
            }
        }
    }
}