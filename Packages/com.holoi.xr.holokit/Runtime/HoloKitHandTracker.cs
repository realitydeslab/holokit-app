// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
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
                if (HoloKitUtils.IsEditor) { return true; }
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

        private void Update()
        {
            if (!_isActive) { return; }

            if (HoloKitUtils.IsEditor) { return; }

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
