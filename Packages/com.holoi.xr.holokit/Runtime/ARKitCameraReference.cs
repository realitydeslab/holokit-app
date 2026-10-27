// SPDX-License-Identifier: MIT
// ARKit's own camera for the current interface orientation, read straight from the ARSession, so device tests can
// check AR Foundation's camera against it. Development builds only: nothing in the app uses it.
//
// It was the reference of ARKitCameraCompatibility (from the HoloKit Unity SDK), which corrected AR Foundation's
// camera on iOS 27: with ARKit XR 6.4.3 the pose lacked the interface roll, 90 degrees off in portrait. ARKit XR 6.7.0
// fixes that (the native plug-in no longer reads the deprecated statusBarOrientation), so the correction is gone.
#if UNITY_IOS && (UNITY_EDITOR || DEVELOPMENT_BUILD)
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace HoloKit.iOS
{
    public static class ARKitCameraReference
    {
        static readonly float[] view = new float[16], projection = new float[16], display = new float[8];
        static readonly List<XRSessionSubsystem> sessions = new();

        /// <summary>
        /// ARKit's camera rotation (local to the XR Origin's camera offset) and projection for the current interface
        /// orientation and screen size, with the camera's clip planes. False before the session has a frame.
        /// </summary>
        public static bool TryGetReference(Camera camera, out Quaternion localRotation, out Matrix4x4 cameraProjection) =>
            TryGetReference(camera, out localRotation, out cameraProjection, out _);

        /// <summary>Also gives the AR background's display transform (the shader's _UnityDisplayTransform).</summary>
        public static bool TryGetReference(Camera camera, out Quaternion localRotation, out Matrix4x4 cameraProjection,
            out Matrix4x4 displayTransform)
        {
            localRotation = Quaternion.identity;
            cameraProjection = Matrix4x4.identity;
            displayTransform = Matrix4x4.identity;
#if UNITY_EDITOR
            return false;
#else
            // Look the session up every time: leaving an AR scene destroys it and the next scene creates a new one.
            SubsystemManager.GetSubsystems(sessions);
            XRSessionSubsystem session = null;
            foreach (var candidate in sessions)
                if (candidate.running) { session = candidate; break; }
            if (session == null) return false;
            if (HoloKit_GetARCameraReference(session.nativePtr, (int)Screen.orientation,
                Screen.width, Screen.height, view, projection, display) == 0) return false;
            var nativeView = new Matrix4x4();
            for (int i=0; i<16; i++) nativeView[i] = view[i];
            var flip = Matrix4x4.Scale(new Vector3(1,1,-1));
            localRotation = (flip * nativeView.inverse * flip).rotation;
            if (camera != null) cameraProjection = ProjectionMatrix(projection, camera.nearClipPlane, camera.farClipPlane);
            displayTransform = DisplayMatrix(display);
            return true;
#endif
        }

        // ARKit's projection for this orientation and viewport, with the camera's own clip planes.
        public static Matrix4x4 ProjectionMatrix(float[] p, float near, float far)
        {
            var result = new Matrix4x4();
            for (int i = 0; i < 16; i++) result[i] = p[i];
            result.m22 = -(far + near) / (far - near);
            result.m23 = -2 * far * near / (far - near);
            return result;
        }

        // ARKit maps image -> UIKit view. Invert, flip viewport V, and
        // transpose for the ARKit shader's row-vector multiplication.
        public static Matrix4x4 DisplayMatrix(float[] d)
        {
            var imageToView = Matrix4x4.identity;
            imageToView.m00 = d[0]; imageToView.m10 = d[1];
            imageToView.m01 = d[2]; imageToView.m11 = d[3];
            imageToView.m03 = d[4]; imageToView.m13 = d[5];
            var inverse = imageToView.inverse;
            var result = Matrix4x4.identity;
            result.m00 = inverse.m00; result.m10 = -inverse.m01;
            result.m20 = inverse.m01 + inverse.m03;
            result.m01 = inverse.m10; result.m11 = -inverse.m11;
            result.m21 = inverse.m11 + inverse.m13;
            return result;
        }

#if !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern int HoloKit_GetARCameraReference(IntPtr session, int orientation, int width, int height,
            [Out] float[] view, [Out] float[] projection, [Out] float[] display);
#endif
    }
}
#endif
