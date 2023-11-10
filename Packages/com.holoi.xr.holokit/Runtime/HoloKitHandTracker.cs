// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;

namespace HoloKit
{
    public enum HoloKitHandJoint
    {
        Wrist = 0,
        Thumb0 = 1,
        Thumb1 = 2,
        Thumb2 = 3,
        Thumb3 = 4,
        Index0 = 5,
        Index1 = 6,
        Index2 = 7,
        Index3 = 8,
        Middle0 = 9,
        Middle1 = 10,
        Middle2 = 11,
        Middle3 = 12,
        Ring0 = 13,
        Ring1 = 14,
        Ring2 = 15,
        Ring3 = 16,
        Little0 = 17,
        Little1 = 18,
        Little2 = 19,
        Little3 = 20
    }

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
                if(HoloKitUtils.IsEditor) { return true; }
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
                HoloKitHandJoint joint = (HoloKitHandJoint)i;
                switch (joint)
                {
                    case HoloKitHandJoint.Wrist:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.red;
                        break;
                    case HoloKitHandJoint.Thumb0:
                    case HoloKitHandJoint.Index0:
                    case HoloKitHandJoint.Middle0:
                    case HoloKitHandJoint.Ring0:
                    case HoloKitHandJoint.Little0:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
                        break;
                    case HoloKitHandJoint.Thumb1:
                    case HoloKitHandJoint.Index1:
                    case HoloKitHandJoint.Middle1:
                    case HoloKitHandJoint.Ring1:
                    case HoloKitHandJoint.Little1:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                    case HoloKitHandJoint.Thumb2:
                    case HoloKitHandJoint.Index2:
                    case HoloKitHandJoint.Middle2:
                    case HoloKitHandJoint.Ring2:
                    case HoloKitHandJoint.Little2:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
                        break;
                    case HoloKitHandJoint.Thumb3:
                    case HoloKitHandJoint.Index3:
                    case HoloKitHandJoint.Middle3:
                    case HoloKitHandJoint.Ring3:
                    case HoloKitHandJoint.Little3:
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

        public Vector3 GetHandJointPosition(HoloKitHandJoint joint)
        {
            return _handJoints[(int)joint].position;
        }
    }
}
