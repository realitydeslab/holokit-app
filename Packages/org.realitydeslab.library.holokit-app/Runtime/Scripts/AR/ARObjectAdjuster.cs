// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp.UI;
using HoloKit;

namespace RealityDesignLab.Library.HoloKitApp
{
    /// <summary>
    /// This class is a reactor for adjustment in reality settings.
    /// Adjustment means translating, rotating and scaling.
    /// This class is a singleton.
    /// </summary>
    public class ARObjectAdjuster : NetworkBehaviour
    {
        public static ARObjectAdjuster Instance { get { return _instance; } }

        private static ARObjectAdjuster _instance;

        [SerializeField] private bool _translation = true;

        [SerializeField] private bool _rotation = true;

        [SerializeField] private bool _scale = true;

        /// <summary>
        /// The AR object you want to adjust.
        /// </summary>
        [SerializeField] private Transform _arObject;

        public bool Translation => _translation;

        public bool Rotation => _rotation;

        public bool Scale => _scale;

        private const float TranslationSpeed = 0.002f;

        private const float RotationSpeed = 1.6f;

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
        }

        private void Start()
        {
            if (_translation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnPositionChanged += OnPositionChanged;
            }
            if (_rotation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnRotationChanged += OnRotationChanged;
            }
            if (_scale)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnScaleChanged += OnScaleChanged;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_translation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnPositionChanged -= OnPositionChanged;
            }
            if (_rotation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnRotationChanged -= OnRotationChanged;
            }
            if (_scale)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnScaleChanged -= OnScaleChanged;
            }
        }

        public void SetARObject(Transform arObject)
        {
            _arObject = arObject;
        }

        private void OnPositionChanged(Vector2 offset)
        {
            if (_arObject != null)
            {
                OnPositionChangedServerRpc(offset);
            }
        }

        private void OnRotationChanged(float angle)
        {
            if (_arObject != null)
            {
                OnRotationChangedServerRpc(angle);
            }
        }

        private void OnScaleChanged(float factor)
        {
            if (_arObject != null)
            {
                OnScaleChangedServerRpc(factor);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnPositionChangedServerRpc(Vector2 offset)
        {
            Vector3 forward = HoloKitCameraManager.Instance.CenterEyePose.forward;
            Vector3 horizontalForward = new(forward.x, 0f, forward.z);
            Vector3 right = HoloKitCameraManager.Instance.CenterEyePose.right;
            Vector3 horizontalRight = new(right.x, 0f, right.z);
            _arObject.position += TranslationSpeed * (offset.x * horizontalRight + offset.y * horizontalForward);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnRotationChangedServerRpc(float angle)
        {
            _arObject.Rotate(0f, -RotationSpeed * angle, 0f);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnScaleChangedServerRpc(float factor)
        {
            _arObject.localScale *= factor;
        }
    }
}
