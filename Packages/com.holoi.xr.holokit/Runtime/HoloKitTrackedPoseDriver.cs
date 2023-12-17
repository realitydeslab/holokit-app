// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;
using System.Runtime.InteropServices;

namespace HoloKit
{
    [RequireComponent(typeof(ARCameraManager))]
    [RequireComponent(typeof(TrackedPoseDriver))]
    [RequireComponent(typeof(HoloKitCameraManager))]
    [DisallowMultipleComponent]
    public class HoloKitTrackedPoseDriver : MonoBehaviour
    {
        private TrackedPoseDriver _monoTrackedPoseDriver;

        private InputDevice _inputDevice;

        private ARCameraManager _arCameraManager;

        private HoloKitCameraManager _holoKitCameraManager; 

        private IntPtr _headTrackerPtr;

        private void Start()
        {
            _arCameraManager = GetComponent<ARCameraManager>();
            if (_arCameraManager == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find ARCameraManager");
                return;
            }

            _monoTrackedPoseDriver = GetComponent<TrackedPoseDriver>();
            if (_monoTrackedPoseDriver == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find TrackedPoseDriver");
                return;
            }

            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count > 0)
                _inputDevice = devices[0];
            if (_inputDevice == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find InputDevice");
                return;
            }

            _holoKitCameraManager = GetComponent<HoloKitCameraManager>();
            if (_holoKitCameraManager == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find HoloKitCamera");
                return;
            }

            if (_holoKitCameraManager == null)
            {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find HoloKitCamera");
                return;
            }

            if (_holoKitCameraManager.CenterEyePose == null) {
                Debug.LogWarning("[HoloKitTrackedPoseDriver] Failed to find CenterEyePose from HoloKitCamera");
                return;
            }

            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;

            _arCameraManager.frameReceived += OnFrameReceived;

            Application.onBeforeRender += OnBeforeRender;

        #if UNITY_IOS && !UNITY_EDITOR
            _headTrackerPtr = Init();
            InitHeadTracker(_headTrackerPtr);
            PauseHeadTracker(_headTrackerPtr);
        #endif
        }

        private void Awake()
        {
        }

        private void OnBeforeRender()
        {
            if (_holoKitCameraManager.RenderMode == HoloKitRenderMode.Mono) {
                return;
            }

            UpdateHeadTrackerPose();
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
            _arCameraManager.frameReceived -= OnFrameReceived;
            Application.onBeforeRender -= OnBeforeRender;
        #if UNITY_IOS && !UNITY_EDITOR
            if (_headTrackerPtr != IntPtr.Zero) {
                Delete(_headTrackerPtr);
            }
        #endif
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
        #if UNITY_IOS && !UNITY_EDITOR
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                ResumeHeadTracker(_headTrackerPtr);
            }
            else
            {
                PauseHeadTracker(_headTrackerPtr);
            }
        #endif
        }

        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (_holoKitCameraManager.RenderMode == HoloKitRenderMode.Mono) {
                return;
            }
        #if UNITY_IOS && !UNITY_EDITOR
            bool isPositionValid = _inputDevice.TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 position) || _inputDevice.TryGetFeatureValue(CommonUsages.colorCameraPosition, out position);
            bool isRotationValid = _inputDevice.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion rotation) || _inputDevice.TryGetFeatureValue(CommonUsages.colorCameraRotation, out rotation);

            if (isPositionValid && isRotationValid)
            {
                float[] positionArr = new float[] { position.x, position.y, position.z };
                float[] rotationArr = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
                AddSixDoFData(_headTrackerPtr, (long) args.timestampNs, positionArr, rotationArr);
            }
        #endif
        }

        private void UpdateHeadTrackerPose()
        {
            if (_holoKitCameraManager.RenderMode == HoloKitRenderMode.Mono) {
                return;
            }
        #if UNITY_IOS && !UNITY_EDITOR
            float[] positionArr = new float[3];
            float[] rotationArr = new float[4];

            
            GetHeadTrackerPose(_headTrackerPtr, positionArr, rotationArr);

            Vector3 position = new(positionArr[0], positionArr[1], positionArr[2]);
            Quaternion rotation = new(rotationArr[0], rotationArr[1], rotationArr[2], rotationArr[3]);

            _holoKitCameraManager.CenterEyePose.position = position;
            _holoKitCameraManager.CenterEyePose.rotation = rotation;
        #endif
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
    }
}
