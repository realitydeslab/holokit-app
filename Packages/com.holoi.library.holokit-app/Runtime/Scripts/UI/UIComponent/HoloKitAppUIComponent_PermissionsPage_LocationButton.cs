// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_LocationButton : HoloKitAppUITemplate_PermissionsPage_PermissionButton
    {
        private void Awake()
        {
            PermissionsAPI.OnLocationPermissionStatusChanged += OnLocationPermissionStatusChanged;
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnLocationPermissionStatusChanged -= OnLocationPermissionStatusChanged;
        }

        private void OnLocationPermissionStatusChanged(LocationPermissionStatus status)
        {
            switch (status)
            {
                case LocationPermissionStatus.NotDetermined:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.NotDetermined);
                    break;
                case LocationPermissionStatus.Restricted:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
                    break;
                case LocationPermissionStatus.Denied:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
                    break;
                case LocationPermissionStatus.AuthorizedAlways:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
                    break;
                case LocationPermissionStatus.AuthorizedWhenInUse:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
                    break;
                case LocationPermissionStatus.Authorized:
                    UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
                    break;
            }
        }

        protected override HoloKitAppPermissionStatus GetPermissionStatus()
        {
            return HoloKitAppPermissionsManager.GetLocationPermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestLocationWhenInUsePermission();
        }
    }
}
