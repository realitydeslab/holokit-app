// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HoloKit
{
    public static class HoloKitNFCSessionControllerAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitSDK_StartNFCSession(string alertMessage, int holoKitType, float ipd, float farClipPlane);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SkipNFCSessionWithPassword(string password, int holoKitType, float ipd, float farClipPlane);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_RegisterNFCSessionControllerDelegates(Action<bool, IntPtr> OnNFCSessionCompleted);

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void OnNFCSessionCompletedDelegate(bool success, IntPtr cameraDataPtr)
        {
            if (success && HoloKitUtils.IsRuntime)
            {
                HoloKitCameraData cameraData = ParseHoloKitCameraData(cameraDataPtr);
                if (HoloKitCameraManager.Instance != null)
                {
                    HoloKitCameraManager.Instance.SetupHoloKitCameraData(cameraData);
                }
            }
            OnNFCSessionCompleted?.Invoke(success);
        }

        public static event Action<bool> OnNFCSessionCompleted;

        public static void StartNFCSession(HoloKitType holoKitType, float ipd, float farClipPlane)
        {
            if (HoloKitUtils.IsRuntime)
            {
                string alertMessage = "Please put your iPhone onto the HoloKit";
                HoloKitSDK_StartNFCSession(alertMessage, (int)holoKitType, ipd, farClipPlane);
            }
        }

        public static void SkipNFCSessionWithPassword(string password, HoloKitType holoKitType, float ipd, float farClipPlane)
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitSDK_SkipNFCSessionWithPassword(password, (int)holoKitType, ipd, farClipPlane);
            }
            else
            {
                OnNFCSessionCompletedDelegate(true, (IntPtr)null);
            }
        }

        public static void RegisterNFCSessionControllerDelegates()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitSDK_RegisterNFCSessionControllerDelegates(OnNFCSessionCompletedDelegate);
            }
        }

        private static HoloKitCameraData ParseHoloKitCameraData(IntPtr cameraDataPtr)
        {
            float[] result = new float[55];
            Marshal.Copy(cameraDataPtr, result, 0, 55);
            Rect leftViewportRect = Rect.MinMaxRect(result[0], result[1], result[2], result[3]);
            Rect rightViewportRect = Rect.MinMaxRect(result[4], result[5], result[6], result[7]);
            Matrix4x4 leftProjectionMatrix = Matrix4x4.zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    leftProjectionMatrix[i, j] = result[10 + 4 * i + j];
                }
            }
            Matrix4x4 rightProjectionMatrix = Matrix4x4.zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rightProjectionMatrix[i, j] = result[26 + 4 * i + j];
                }
            }
            return new HoloKitCameraData
            {
                LeftViewportRect = leftViewportRect,
                RightViewportRect = rightViewportRect,
                NearClipPlane = result[8],
                FarClipPlane = result[9],
                LeftProjectionMatrix = leftProjectionMatrix,
                RightProjectionMatrix = rightProjectionMatrix,
                CameraToCenterEyeOffset = new Vector3(result[42], result[43], result[44]),
                CameraToScreenCenterOffset = new Vector3(result[45], result[46], result[47]),
                CenterEyeToLeftEyeOffset = new Vector3(result[48], result[49], result[50]),
                CenterEyeToRightEyeOffset = new Vector3(result[51], result[52], result[53]),
                AlignmentMarkerOffset = result[54]
            };
        }
    }
}