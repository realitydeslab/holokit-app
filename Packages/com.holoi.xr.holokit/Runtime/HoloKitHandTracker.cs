// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-FileContributor: Botao 'Amber' Hu <amber@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;

namespace HoloKit
{
    // https://developer.apple.com/documentation/vision/vnhumanhandposeobservationjointname?language=objc
    public enum HandJoint
    {
         // The wrist.
        Wrist = 0,
        // The thumb’s carpometacarpal (CMC) joint.
        ThumbCMC = 1,
        //The thumb’s metacarpophalangeal (MP) joint.
        ThumbMP = 2,
        // The thumb’s interphalangeal (IP) joint.
        ThumbIP = 3,
        // The tip of the thumb.
        ThumbTip = 4,
        // The index finger’s metacarpophalangeal (MCP) joint.
        IndexMCP = 5,
        // The index finger’s proximal interphalangeal (PIP) joint.
        IndexPIP = 6,
        // The index finger’s distal interphalangeal (DIP) joint.
        IndexDIP = 7,
        // The tip of the index finger.
        IndexTip = 8,
        // The middle finger’s metacarpophalangeal (MCP) joint.
        MiddleMCP = 9,
        // The middle finger’s proximal interphalangeal (PIP) joint.
        MiddlePIP = 10,
        // The middle finger’s distal interphalangeal (DIP) joint.
        MiddleDIP = 11,
        // The tip of the middle finger.
        MiddleTip = 12,
        // The ring finger’s metacarpophalangeal (MCP) joint.
        RingMCP = 13,
        // The ring finger’s proximal interphalangeal (PIP) joint.
        RingPIP = 14,
        // The ring finger’s distal interphalangeal (DIP) joint.
        RingDIP = 15,
        // The tip of the ring finger.
        RingTip = 16,
        // The little finger’s metacarpophalangeal (MCP) joint.
        LittleMCP = 17,
        // The little finger’s proximal interphalangeal (PIP) joint.
        LittlePIP = 18,
        // The little finger’s distal interphalangeal (DIP) joint.
        LittleDIP = 19,
        // The tip of the little finger.
        LittleTip = 20
    }

    [DisallowMultipleComponent]
    public class HoloKitHandTracker : MonoBehaviour
    {
        public static HoloKitHandTracker Instance { get { return _instance; } }

        private static HoloKitHandTracker _instance;

        [SerializeField] private bool _isActive;

        [SerializeField] private bool _isVisible;

        [SerializeField] private float _fadeOutDelay = 1.2f;

        [SerializeField] private GameObject _hand;

        [SerializeField] private Transform[] _handJoints;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    HoloKitHandTrackingControllerAPI.SetHandTrackingActive(_isActive);
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    SetHandJointsVisible(value);
                } 
            }
        }

        public bool IsValid
        {
            get
            {
                // In the Editor there is no camera to find a hand; once a test injects poses it behaves as on a device.
                if (HoloKitUtils.IsEditor && !_injectedInEditor) { return true; }
                return _isValid;
            }
        }

        private bool _isValid = false;

        private float _lastUpdateTime;

        public static event Action<bool> OnHandValidityChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
            HoloKitHandTrackingControllerAPI.SetHandTrackingActive(_isActive);
        }

        private void Start()
        {
            HoloKitHandTrackingControllerAPI.OnHandPoseUpdated += OnHandPoseUpdated;
            HoloKitHandTrackingControllerAPI.RegisterHandTrackingControllerDelegates();
            SetupHandJointColors();
            SetHandJointsVisible(_isVisible);

            if (HoloKitUtils.IsEditor)
            {
                _isValid = true;
            }
        }

        private void OnDestroy()
        {
            HoloKitHandTrackingControllerAPI.OnHandPoseUpdated -= OnHandPoseUpdated;
            HoloKitHandTrackingControllerAPI.SetHandTrackingActive(false);
        }

        private void SetupHandJointColors()
        {
            for (int i = 0; i < 21; i++)
            {
                HandJoint joint = (HandJoint)i;
                switch (joint)
                {
                    case HandJoint.Wrist:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.red;
                        break;
                    case HandJoint.ThumbCMC:
                    case HandJoint.IndexMCP:
                    case HandJoint.MiddleMCP:
                    case HandJoint.RingMCP:
                    case HandJoint.LittleMCP:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
                        break;
                    case HandJoint.ThumbMP:
                    case HandJoint.IndexPIP:
                    case HandJoint.MiddlePIP:
                    case HandJoint.RingPIP:
                    case HandJoint.LittlePIP:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                    case HandJoint.ThumbIP:
                    case HandJoint.IndexDIP:
                    case HandJoint.MiddleDIP:
                    case HandJoint.RingDIP:
                    case HandJoint.LittleDIP:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
                        break;
                    case HandJoint.ThumbTip:
                    case HandJoint.IndexTip:
                    case HandJoint.MiddleTip:
                    case HandJoint.RingTip:
                    case HandJoint.LittleTip:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.blue;
                        break;
                }
            }
        }

        private void SetHandJointsVisible(bool visible)
        {
            for (int i = 0; i < 21; i++)
            {
                _handJoints[i].GetComponent<MeshRenderer>().enabled = visible;
            }
        }

        private void OnHandPoseUpdated(float[] poses)
        {
            _lastUpdateTime = Time.time;
            if (!_isValid)
            {
                _isValid = true;
                _hand.SetActive(true);
                OnHandValidityChanged?.Invoke(true);
            }
            for (int i = 0; i < 21; i++)
            {
                _handJoints[i].position = new Vector3(poses[i * 3], poses[i * 3 + 1], poses[i * 3 + 2]);
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Drives the same pose/validity path as Vision for opt-in device tests.
        // This does not validate camera-based hand detection itself.
        public void InjectPoseForDeviceTest(Vector3[] joints)
        {
            if (joints == null || joints.Length != 21)
                throw new ArgumentException("A hand pose must contain 21 joints.", nameof(joints));
            var poses = new float[63];
            for (var i = 0; i < 21; i++)
            {
                poses[i * 3] = joints[i].x;
                poses[i * 3 + 1] = joints[i].y;
                poses[i * 3 + 2] = joints[i].z;
            }
            // In the Editor the tracker starts out valid (there is no camera to find a hand), so injected poses would
            // never raise OnHandValidityChanged; let the first one appear the way a detected hand does on a device.
            if (HoloKitUtils.IsEditor && !_injectedInEditor)
            {
                _injectedInEditor = true;
                _isValid = false;
            }
            OnHandPoseUpdated(poses);
        }
#endif

        // Set by the first pose a test injects in the Editor; from then on the hand appears and fades out as on a device.
        private bool _injectedInEditor;

        private void Update()
        {
            if (!_isActive) { return; }

            if (HoloKitUtils.IsEditor && !_injectedInEditor) { return; }

            if (_isValid)
            {
                if (Time.time - _lastUpdateTime > _fadeOutDelay)
                {
                    _isValid = false;
                    _hand.SetActive(false);
                    OnHandValidityChanged?.Invoke(false);
                }
            }
        }

        public Vector3 GetHandJointPosition(HandJoint joint)
        {
            return _handJoints[(int)joint].position;
        }
    }
}
