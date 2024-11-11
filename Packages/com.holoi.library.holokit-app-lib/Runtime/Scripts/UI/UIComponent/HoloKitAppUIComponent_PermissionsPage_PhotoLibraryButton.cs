// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using RealityDesignLab.Library.Permissions;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_PhotoLibraryButton : HoloKitAppUITemplate_PermissionsPage_PermissionButton
    {
        private void Awake()
        {
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted += OnRequestPhotoLibraryAddPermissionCompleted;
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted -= OnRequestPhotoLibraryAddPermissionCompleted;
        }

        private void OnRequestPhotoLibraryAddPermissionCompleted(PhotoLibraryPermissionStatus status)
        {
            switch (status)
            {
                case PhotoLibraryPermissionStatus.NotDetermined:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
                    break;
                case PhotoLibraryPermissionStatus.Restricted:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
                    break;
                case PhotoLibraryPermissionStatus.Denied:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
                    break;
                case PhotoLibraryPermissionStatus.Granted:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
                    break;
                case PhotoLibraryPermissionStatus.Limited:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
                    break;
            }
        }

        protected override HoloKitAppPermissionStatus GetPermissionStatus()
        {
            return HoloKitAppPermissionsManager.GetPhotoLibraryAddPermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestPhotoLibraryAddPermission();
        }
    }
}
