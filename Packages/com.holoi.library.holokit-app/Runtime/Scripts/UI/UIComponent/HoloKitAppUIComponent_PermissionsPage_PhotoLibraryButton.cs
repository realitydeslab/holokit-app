// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp.UI
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
