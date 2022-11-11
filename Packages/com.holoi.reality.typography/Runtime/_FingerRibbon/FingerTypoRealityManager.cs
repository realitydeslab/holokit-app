using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class FingerTypoRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [Header("Finger Ribbon")]
        [SerializeField] private List<Transform> _softTips;

        [SerializeField] private GameObject _fingerVfxManager;

        private NetworkVariable<bool> _isHostHandValid = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.enabled = true;
                HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
                HoloKitHandTracker.Instance.IsActive = true;
                foreach (var tip in _softTips)  
                {
                    tip.GetComponent<FollowMovementManager>().enabled = true;
                }
            }
            else
            {
                _arOcclusionManager.enabled = false;
                HoloKitHandTracker.Instance.IsActive = false;
                foreach (var tip in _softTips)
                {
                    tip.GetComponent<FollowMovementManager>().enabled = false;
                }
            }
            _fingerVfxManager.SetActive(false);
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

        private void OnHandValidityChanged(bool isValid)
        {
            _isHostHandValid.Value = isValid;
        }

        private void OnIsHostHandValidValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                _fingerVfxManager.SetActive(true);
            }
            else if (oldValue && !newValue)
            {
                _fingerVfxManager.SetActive(false);
            }
        }
    }
}