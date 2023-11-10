// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;

namespace HoloKit
{
    [RequireComponent(typeof(ARCameraManager))]
    [RequireComponent(typeof(TrackedPoseDriver))]
    [RequireComponent(typeof(HoloKitCamera))]
    public class HoloKitTrackedPoseDriver : MonoBehaviour
    {
        private TrackedPoseDriver m_TrackedPoseDriver;

        private InputDevice m_InputDevice;

        private ARCameraManager m_ARCameraManager;

        private HoloKitCamera m_HoloKitCamera; 

        private IntPtr m_HeadTrackerPtr;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
            }
        }

        /// <summary>
        /// If this value is set to true, HoloKitTrackedPoseDriver will take control
        /// of the camera pose.
        /// </summary>
        private bool _isActive = false;

        private readonly Matrix4x4 RotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));

        private void Start()
        {
            m_ARCameraManager = GetComponent<ARCameraManager>();
            if (m_ARCameraManager == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find ARCameraManager");
                return;
            }

            m_TrackedPoseDriver = GetComponent<TrackedPoseDriver>();
            if (m_TrackedPoseDriver == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find TrackedPoseDriver");
                return;
            }

            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count > 0)
                m_InputDevice = devices[0];
            if (m_InputDevice == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find InputDevice");
                return;
            }

            m_HoloKitCamera = GetComponent<HoloKitCamera>();
            if (m_HoloKitCamera == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find HoloKitCamera");
                return;
            }

#if UNITY_IOS && !UNITY_EDITOR
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;

            m_ARCameraManager.frameReceived += OnFrameReceived;

            Application.onBeforeRender += OnBeforeRender;

            m_HeadTrackerPtr = Init();
            InitHeadTracker(m_HeadTrackerPtr);
            PauseHeadTracker(m_HeadTrackerPtr);
#endif
        }

        private void OnBeforeRender()
        {
            if (m_TrackedPoseDriver != null && !m_TrackedPoseDriver.enabled) 
            {
                UpdateHeadTrackerPose();
            }
        }

        private void Awake()
        {
            // HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame;
        }

        private void OnDestroy()
        {
            // HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame;

#if UNITY_IOS && !UNITY_EDITOR
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
            m_ARCameraManager.frameReceived -= OnFrameReceived;
            Application.onBeforeRender -= OnBeforeRender;
            Delete(m_HeadTrackerPtr);
#endif
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                if (m_TrackedPoseDriver != null) {
                    m_TrackedPoseDriver.enabled = false;
                }
                ResumeHeadTracker(m_HeadTrackerPtr);
            }
            else
            {
                if (m_TrackedPoseDriver != null) {
                    m_TrackedPoseDriver.enabled = true;
                }
                PauseHeadTracker(m_HeadTrackerPtr);
            }
        }

        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (m_TrackedPoseDriver != null && m_TrackedPoseDriver.enabled) {
                return;
            }

            bool isPositionValid = m_InputDevice.TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 position) || m_InputDevice.TryGetFeatureValue(CommonUsages.colorCameraPosition, out position);
            bool isRotationValid = m_InputDevice.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion rotation) || m_InputDevice.TryGetFeatureValue(CommonUsages.colorCameraRotation, out rotation);

            if (isPositionValid && isRotationValid)
            {
                float[] positionArr = new float[] { position.x, position.y, position.z };
                float[] rotationArr = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
                AddSixDoFData(m_HeadTrackerPtr, (long) args.timestampNs, positionArr, rotationArr);
            }
        }

        // private void OnARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        // {
        //     if (_isActive)
        //     {
        //         if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        //             matrix *= RotationMatrix;
        //         transform.SetPositionAndRotation(matrix.GetPosition(), matrix.rotation);
        //     }
        // }

#if UNITY_IOS
        private void UpdateHeadTrackerPose()
        {
            float[] positionArr = new float[3];
            float[] rotationArr = new float[4];

            GetHeadTrackerPose(m_HeadTrackerPtr, positionArr, rotationArr);
            Vector3 position = new(positionArr[0], positionArr[1], positionArr[2]);
            Quaternion rotation = new(rotationArr[0], rotationArr[1], rotationArr[2], rotationArr[3]);

            transform.position = position;
            transform.rotation = rotation;
        }

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_init")]
        static extern IntPtr Init();

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_initHeadTracker")]
        static extern void InitHeadTracker(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_pauseHeadTracker")]
        static extern void PauseHeadTracker(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_resumeHeadTracker")]
        static extern void ResumeHeadTracker(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_addSixDoFData")]
        static extern void AddSixDoFData(IntPtr self, long timestamp, [In] float[] position, [In] float[] orientation);

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_getHeadTrackerPose")]
        static extern void GetHeadTrackerPose(IntPtr self, [Out] float[] position, [Out] float[] orientation);

        [DllImport("__Internal", EntryPoint = "HoloKit_LowLatencyTracking_delete")]
        static extern void Delete(IntPtr self);
#endif
    }
}
