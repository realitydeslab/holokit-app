using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using HoloKit;

using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFAThePuppetry
{
    public class PuppetryRealityManager : RealityManager
    {
        [Header("Editor Debugger")]
        [SerializeField] bool _start = false;

        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] Transform _serverCenterEye;
        [SerializeField] Transform _localCenterEye;
        [SerializeField] GameObject _arSoftShadowPlane;
        [Header("Reality Objects")]
        [SerializeField] GameObject _puppetryPrefab;
        GameObject _puppetryInstance;
        bool _isTriggered = false;

        [Header("UI")]
        [SerializeField] GameObject _starTip;
        [Header("UI Button List")]
        [SerializeField] MyButton _forwardButton;
        [SerializeField] MyButton _backwardButton;
        [SerializeField] MyButton _rightwardButton;
        [SerializeField] MyButton _leftwardButton;

        float _vZ;
        float _vX;

        // ray cast manager
        ARRaycastManager _arRaycastManager;

        ARPlaneManager _arPlaneManager;

        bool _isRaycastHitFloor = false;

        bool _isFloorHeightValid = false;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
#if Unity_IOS
            if (Time.time > 4f && _isFloorHeightValid)
            {
#endif
#if UNITY_EDITOR
            if (_start)
            {
#endif
                if (HoloKitApp.Instance.IsHost)
                {

                }
                else
                {
                    // update controller buttons' state:
                    if (_forwardButton.buttonPressed)
                    {
                        OnForwardButtonStateChangedServerRpc(true);
                    }
                    else
                    {
                        OnForwardButtonStateChangedServerRpc(false);
                    }
                    if (_backwardButton.buttonPressed)
                    {
                        OnBackwardButtonStateChangedServerRpc(true);
                    }
                    else
                    {
                        OnBackwardButtonStateChangedServerRpc(false);
                    }
                    if (_rightwardButton.buttonPressed)
                    {
                        OnRightwardButtonStateChangedServerRpc(true);
                    }
                    else
                    {
                        OnRightwardButtonStateChangedServerRpc(false);
                    }
                    if (_leftwardButton.buttonPressed)
                    {
                        OnLeftwardButtonStateChangedServerRpc(true);
                    }
                    else
                    {
                        OnLeftwardButtonStateChangedServerRpc(false);
                    }
                }

                // update velocity via controller buttons' state:
                UpdatePuppetryVelocity();

                // update floor height:
                UpdateFloorHeight();

                // start experience:
                SetPuppetryAndStartExperienceServerRpc();
            }
        }

        [ServerRpc]
        private void SetPuppetryAndStartExperienceServerRpc()
        {
            if (!_isTriggered)
            {
                // disable tips:
                _starTip.SetActive(false);

                // update plane material to support shadow:
                foreach (var plane in _arPlaneManager.trackables)
                {
                    plane.GetComponent<MeshRenderer>().material = _arSoftShadowPlane.GetComponent<MeshRenderer>().material;
                }

                // initial the avatar
                _puppetryInstance = Instantiate(_puppetryPrefab);
                _puppetryInstance.transform.position = HitPosition;
                var lookAtPos = new Vector3(_localCenterEye.position.x, HitPosition.y, _localCenterEye.position.z);
                _puppetryInstance.transform.LookAt(lookAtPos);
                // Get the instance's NetworkObject and Spawn
                var puppetryNetWorkObject = _puppetryInstance.GetComponent<NetworkObject>();
                puppetryNetWorkObject.Spawn();
                _isTriggered = true;
            }
        }

        void UpdateFloorHeight()
        {

            Vector3 rayOrigin = _centerEye.position + _centerEye.forward * 1f;

            Ray ray = new(rayOrigin, Vector3.down);

            List<ARRaycastHit> hitResults = new();

            if (_arRaycastManager.Raycast(ray, hitResults, TrackableType.Planes))
            {
                foreach (var hitResult in hitResults)
                {
                    var arPlane = hitResult.trackable.GetComponent<ARPlane>();

                    if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                    {
                        HitPosition = hitResult.pose.position;
                        transform.position = HitPosition;
                        _isRaycastHitFloor = true;
                        _isFloorHeightValid = true;
                        return;
                    }
                }
                _isRaycastHitFloor = false;

            }
            else
            {
                _isRaycastHitFloor = false;
            }
        }

        void UpdatePuppetryVelocity()
        {
            if (_forwardButton.buttonPressed)
            {
                Debug.Log("_forwardButton");
                _vZ = 1.51f * Time.deltaTime;
            }
            else if (_backwardButton.buttonPressed)
            {
                Debug.Log("_backwardButton");

                _vZ = -1.51f * Time.deltaTime;

            }
            else if (_rightwardButton.buttonPressed)
            {
                Debug.Log("_rightwardButton");

                _vX = 1.51f * Time.deltaTime;
            }
            else if (_leftwardButton.buttonPressed)
            {
                Debug.Log("_leftwardButton");

                _vX = -1.51f * Time.deltaTime;
            }
        }

        [ServerRpc]
        public void OnForwardButtonStateChangedServerRpc(bool state)
        {
            Debug.Log("OnForwardButtonStateChangedServerRpc");
            _forwardButton.buttonPressed = state;
        }
        [ServerRpc]
        public void OnBackwardButtonStateChangedServerRpc(bool state)
        {
            Debug.Log("OnBackwardButtonStateChangedServerRpc");

            _forwardButton.buttonPressed = state;

        }
        [ServerRpc]
        public void OnLeftwardButtonStateChangedServerRpc(bool state)
        {
            Debug.Log("OnLeftwardButtonStateChangedServerRpc");

            _forwardButton.buttonPressed = state;

        }
        [ServerRpc]
        public void OnRightwardButtonStateChangedServerRpc(bool state)
        {
            Debug.Log("OnRightwardButtonStateChangedServerRpc");

            _forwardButton.buttonPressed = state;

        }
    }
}