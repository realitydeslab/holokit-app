using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppPermissionsManager
    {
        public static bool AreMandatoryPermissionsGranted()
        {
            if (PermissionsAPI.GetCameraPermissionStatus() != CameraPermissionStatus.Granted)
            {
                return false;
            }
            if (PermissionsAPI.GetMicrophonePermissionStatus() != MicrophonePermissionStatus.Granted)
            {
                return false;
            }
            if (PermissionsAPI.GetPhotoLibraryAddPermissionStatus() != PhotoLibraryPermissionStatus.Granted)
            {
                return false;
            }
            if (PermissionsAPI.GetLocationPermissionStatus() != LocationPermissionStatus.AuthorizedWhenInUse)
            {
                return false;
            }
            return true;
        }
    }
}
