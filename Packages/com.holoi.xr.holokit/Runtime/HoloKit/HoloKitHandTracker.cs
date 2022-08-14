using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloKit
{
    public enum HandJoint
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

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                EnableAROcclusionManager(_active);
                HoloKitHandTrackingControllerAPI.SetHandTrackingActive(_active);
            }
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                SetHandJointsVisible(value);
            }
        }

        public bool Valid
        {
            get
            {
                if (_hand.activeSelf)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [SerializeField] private bool _active;

        [SerializeField] private bool _visible;

        [SerializeField] private float _fadeOutDelay = 1.2f;

        [SerializeField] private GameObject _hand;

        [SerializeField] private Transform[] _handJoints;

        private float _lastUpdateTime;

        public static event Action<bool> OnHandValidityChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            HoloKitHandTrackingControllerAPI.OnHandPoseUpdated += OnHandPoseUpdated;
            HoloKitHandTrackingControllerAPI.RegisterHandTrackingControllerDelegates();
            EnableAROcclusionManager(_active);
            HoloKitHandTrackingControllerAPI.SetHandTrackingActive(_active);
            SetupHandJointColors();
            SetHandJointsVisible(_visible);
        }

        private void OnDestroy()
        {
            HoloKitHandTrackingControllerAPI.OnHandPoseUpdated -= OnHandPoseUpdated;
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
                    case HandJoint.Thumb0:
                    case HandJoint.Index0:
                    case HandJoint.Middle0:
                    case HandJoint.Ring0:
                    case HandJoint.Little0:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
                        break;
                    case HandJoint.Thumb1:
                    case HandJoint.Index1:
                    case HandJoint.Middle1:
                    case HandJoint.Ring1:
                    case HandJoint.Little1:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                    case HandJoint.Thumb2:
                    case HandJoint.Index2:
                    case HandJoint.Middle2:
                    case HandJoint.Ring2:
                    case HandJoint.Little2:
                        _handJoints[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
                        break;
                    case HandJoint.Thumb3:
                    case HandJoint.Index3:
                    case HandJoint.Middle3:
                    case HandJoint.Ring3:
                    case HandJoint.Little3:
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
            if (!_hand.activeSelf)
            {
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
            if (Time.time - _lastUpdateTime > _fadeOutDelay)
            {
                _hand.SetActive(false);
                OnHandValidityChanged?.Invoke(false);
            }
        }

        public Vector3 GetHandJointPosition(HandJoint joint)
        {
            if (!_hand.activeSelf)
            {
                return Vector3.zero;
            }

            return _handJoints[(int)joint].position;
        }

        private void EnableAROcclusionManager(bool enabled)
        {
            var arOcclusionManager = FindObjectOfType<AROcclusionManager>(true);
            if (arOcclusionManager != null)
            {
                arOcclusionManager.enabled = enabled;
            }
        }
    }
}
