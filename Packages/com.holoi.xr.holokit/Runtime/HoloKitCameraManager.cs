// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;

namespace HoloKit
{
    public struct HoloKitCameraData
    {
        public Rect LeftViewportRect;
        public Rect RightViewportRect;
        public float NearClipPlane;
        public float FarClipPlane;
        public Matrix4x4 LeftProjectionMatrix;
        public Matrix4x4 RightProjectionMatrix;
        public Vector3 CameraToCenterEyeOffset;
        public Vector3 CameraToScreenCenterOffset;
        public Vector3 CenterEyeToLeftEyeOffset;
        public Vector3 CenterEyeToRightEyeOffset;
        // The horizontal distance from the screen center in pixels
        public float AlignmentMarkerOffset;
    }

    public enum HoloKitRenderMode
    {
        Mono = 0,
        Stereo = 1
    }

    public enum VideoEnhancementMode
    {
        None = 0,      // Default HD
        HighRes = 1,   // 4K
        HighResWithHDR // 4K with HDR
    }

    [RequireComponent(typeof(ARCameraManager))]
    [RequireComponent(typeof(ARCameraBackground))]
    [RequireComponent(typeof(TrackedPoseDriver))]
    [RequireComponent(typeof(HoloKitTrackedPoseDriver))]
    [DisallowMultipleComponent]
    public class HoloKitCameraManager : MonoBehaviour
    {
        public static HoloKitCameraManager Instance { get { return _instance; } }

        private static HoloKitCameraManager _instance;

        [SerializeField] internal Transform _centerEyePose;

        [SerializeField] internal Camera _monoCamera;

        [SerializeField] internal Camera _leftEyeCamera;

        [SerializeField] internal Camera _rightEyeCamera;

        [SerializeField] internal Camera _blackCamera;

        [SerializeField]
        [Range(0.054f, 0.074f)]
        internal float _ipd = 0.064f;

        [SerializeField] internal float _farClipPlane = 50f;

        /// <summary>
        /// This value can only be set before the first ARSession frame.
        /// </summary>
        [SerializeField] private VideoEnhancementMode _videoEnhancementMode = VideoEnhancementMode.None;

        /// <summary>
        /// If this value is set to true, the screen orientation will be set automatically
        /// based on the current render mode. The screen orientation will be set to
        /// Portrait if the current render mode is Mono. The screen orientation will be
        /// set to LandscapeLeft if the current render mode is Stereo.
        /// </summary>
        [SerializeField] private bool _forceScreenOrientation = true;

        public Transform CenterEyePose
        {
            get
            {
                return _centerEyePose;
            }
        }

        public HoloKitRenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                if (_renderMode != value)
                {
                    if (value == HoloKitRenderMode.Stereo)
                    {
                        HoloKitNFCSessionControllerAPI.SkipNFCSessionWithPassword("SomethingForNothing", HoloKitType.HoloKitX, _ipd, _farClipPlane);
                    }
                    else
                    {
                        _renderMode = HoloKitRenderMode.Mono;
                        OnRenderModeChanged();
                        OnHoloKitRenderModeChanged?.Invoke(HoloKitRenderMode.Mono);
                    }
                }
            }
        }

        public VideoEnhancementMode VideoEnhancementMode
        {
            get => _videoEnhancementMode;
            set
            {
                _videoEnhancementMode = value;
            }
        }

        public float AlignmentMarkerOffset => _alignmentMarkerOffset;

        public float ARSessionStartTime => _arSessionStartTime;

        public bool ForceScreenOrientation
        {
            get => _forceScreenOrientation;
            set
            {
                _forceScreenOrientation = value;
            }
        }

        private HoloKitRenderMode _renderMode = HoloKitRenderMode.Mono;

        private float _alignmentMarkerOffset;

        private float _arSessionStartTime;

        private ARCameraBackground _arCameraBackground;

        /// <summary>
        /// The default ARFoundation tracked pose driver. We use this to control
        /// the camera pose in mono mode.
        /// </summary>
        private TrackedPoseDriver _monoTrackedPoseDriver;

        /// <summary>
        /// We use this to control the camera pose in star mode.
        /// </summary>
        private HoloKitTrackedPoseDriver _stereoTrackedPoseDriver;

        /// <summary>
        /// Increase iOS screen brightness gradually in each frame.
        /// </summary>
        private const float ScreenBrightnessIncreaseStep = 0.005f;

        public static event Action<HoloKitRenderMode> OnHoloKitRenderModeChanged;

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
        }

        private void Start()
        {
            // iOS screen system settings
            if (HoloKitUtils.IsRuntime)
            {
                UnityEngine.iOS.Device.hideHomeButton = true;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            // Get the reference of tracked pose drivers
            _monoTrackedPoseDriver = GetComponent<TrackedPoseDriver>();
            _stereoTrackedPoseDriver = GetComponent<HoloKitTrackedPoseDriver>();

            HoloKitNFCSessionControllerAPI.OnNFCSessionCompleted += OnNFCSessionCompleted;
            HoloKitARSessionControllerAPI.ResetARSessionFirstFrame();
            HoloKitARSessionControllerAPI.SetVideoEnhancementMode(_videoEnhancementMode);

            _arCameraBackground = GetComponent<ARCameraBackground>();
            OnRenderModeChanged();
  
            _arSessionStartTime = Time.time;
        }

        private void Update()
        {
            if (_renderMode == HoloKitRenderMode.Stereo)
            {
                if (HoloKitUtils.IsRuntime)
                {
                    // Force screen brightness to be 1 in Stereo mode
                    var screenBrightness = HoloKitARSessionControllerAPI.GetScreenBrightness();
                    if (screenBrightness < 1f)
                    {
                        var newScreenBrightness = screenBrightness + ScreenBrightnessIncreaseStep;
                        if (newScreenBrightness > 1f) newScreenBrightness = 1f;
                        HoloKitARSessionControllerAPI.SetScreenBrightness(newScreenBrightness);
                        HoloKitARSessionControllerAPI.SetScreenBrightness(1f);
                    }
                }

                if (Screen.orientation != ScreenOrientation.LandscapeLeft)
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                if (_forceScreenOrientation)
                {
                    if (Screen.orientation != ScreenOrientation.Portrait)
                        Screen.orientation = ScreenOrientation.Portrait;
                }
            }
        }

        private void OnDestroy()
        {
            HoloKitNFCSessionControllerAPI.OnNFCSessionCompleted -= OnNFCSessionCompleted;
        }

        public void SetupHoloKitCameraData(HoloKitCameraData holokitCameraData)
        {
            _centerEyePose.localPosition = holokitCameraData.CameraToCenterEyeOffset;
            _leftEyeCamera.transform.localPosition = holokitCameraData.CenterEyeToLeftEyeOffset;
            _rightEyeCamera.transform.localPosition = holokitCameraData.CenterEyeToRightEyeOffset;

            // Setup left eye camera
            _leftEyeCamera.nearClipPlane = holokitCameraData.NearClipPlane;
            _leftEyeCamera.farClipPlane = holokitCameraData.FarClipPlane;
            _leftEyeCamera.rect = holokitCameraData.LeftViewportRect;
            _leftEyeCamera.projectionMatrix = holokitCameraData.LeftProjectionMatrix;
            // Setup right eye camera
            _rightEyeCamera.nearClipPlane = holokitCameraData.NearClipPlane;
            _rightEyeCamera.farClipPlane = holokitCameraData.FarClipPlane;
            _rightEyeCamera.rect = holokitCameraData.RightViewportRect;
            _rightEyeCamera.projectionMatrix = holokitCameraData.RightProjectionMatrix;

            _alignmentMarkerOffset = holokitCameraData.AlignmentMarkerOffset;
        }

        private void OnRenderModeChanged()
        {
            if (_renderMode == HoloKitRenderMode.Stereo)
            {
                // Switch ARBackground
                _arCameraBackground.enabled = false;
                // Switch cameras
                _monoCamera.enabled = false;
                _leftEyeCamera.gameObject.SetActive(true);
                _rightEyeCamera.gameObject.SetActive(true);
                _blackCamera.gameObject.SetActive(true);
                _monoTrackedPoseDriver.enabled = true;
            }
            else
            {
                // Switch ARBackground
                _arCameraBackground.enabled = true;
                // Switch cameras
                _monoCamera.enabled = true;
                _leftEyeCamera.gameObject.SetActive(false);
                _rightEyeCamera.gameObject.SetActive(false);
                _blackCamera.gameObject.SetActive(false);
                // Reset center eye pose offset
                _centerEyePose.localPosition = Vector3.zero;
                _monoTrackedPoseDriver.enabled = true;
            }
        }

        private void OnNFCSessionCompleted(bool success)
        {
            if (success)
            {
                _renderMode = HoloKitRenderMode.Stereo;
                OnRenderModeChanged();
                OnHoloKitRenderModeChanged?.Invoke(HoloKitRenderMode.Stereo);
            }
        }

        public void OpenStereoWithoutNFC(string password)
        {
            HoloKitNFCSessionControllerAPI.SkipNFCSessionWithPassword(password, HoloKitType.HoloKitX, _ipd, _farClipPlane);
        }
    }
}
