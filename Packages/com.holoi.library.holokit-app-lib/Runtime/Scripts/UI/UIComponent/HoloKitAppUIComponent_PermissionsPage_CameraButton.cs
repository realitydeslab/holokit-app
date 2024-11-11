// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using RealityDesignLab.Library.Permissions;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_CameraButton : HoloKitAppUITemplate_PermissionsPage_PermissionButton
    {
        private void Awake()
        {
            PermissionsAPI.OnRequestCameraPermissionCompleted += OnRequestCameraPermissionCompleted;
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnRequestCameraPermissionCompleted -= OnRequestCameraPermissionCompleted;
        }

        private void OnRequestCameraPermissionCompleted(bool granted)
        {
            if (granted)
            {
                UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
            }
            else
            {
                UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
            }
        }

        protected override HoloKitAppPermissionStatus GetPermissionStatus()
        {
            return HoloKitAppPermissionsManager.GetCameraPermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestCameraPermission();
        }
    }
}
