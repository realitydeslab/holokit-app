// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_MicrophoneButton : HoloKitAppUITemplate_PermissionsPage_PermissionButton
    {
        private void Awake()
        {
            PermissionsAPI.OnRequestMicrophonePermissionCompleted += OnRequestMicrophonePermissionCompleted;
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnRequestMicrophonePermissionCompleted -= OnRequestMicrophonePermissionCompleted;
        }

        private void OnRequestMicrophonePermissionCompleted(bool granted)
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
            return HoloKitAppPermissionsManager.GetMicrophonePermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestMicrophonePermission();
        }
    }
}
