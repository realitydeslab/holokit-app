using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp
{
    public enum HoloKitAppPermissionStatus
    {
        NotDetermined = 0,
        Denied = 1,
        Granted = 2
    }

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

        public static HoloKitAppPermissionStatus GetCameraPermissionStatus()
        {
            CameraPermissionStatus status = PermissionsAPI.GetCameraPermissionStatus();
            switch (status)
            {
                case CameraPermissionStatus.NotDetermined:
                    return HoloKitAppPermissionStatus.NotDetermined;
                case CameraPermissionStatus.Restricted:
                    return HoloKitAppPermissionStatus.Denied;
                case CameraPermissionStatus.Denied:
                    return HoloKitAppPermissionStatus.Denied;
                case CameraPermissionStatus.Granted:
                    return HoloKitAppPermissionStatus.Granted;
                default:
                    return HoloKitAppPermissionStatus.Denied;
            }
        }

        public static HoloKitAppPermissionStatus GetMicrophonePermissionStatus()
        {
            MicrophonePermissionStatus status = PermissionsAPI.GetMicrophonePermissionStatus();
            switch (status)
            {
                case MicrophonePermissionStatus.NotDetermined:
                    return HoloKitAppPermissionStatus.NotDetermined;
                case MicrophonePermissionStatus.Denied:
                    return HoloKitAppPermissionStatus.Denied;
                case MicrophonePermissionStatus.Granted:
                    return HoloKitAppPermissionStatus.Granted;
                default:
                    return HoloKitAppPermissionStatus.Denied;
            }
        }

        public static HoloKitAppPermissionStatus GetPhotoLibraryAddPermissionStatus()
        {
            PhotoLibraryPermissionStatus status = PermissionsAPI.GetPhotoLibraryAddPermissionStatus();
            switch (status)
            {
                case PhotoLibraryPermissionStatus.NotDetermined:
                    return HoloKitAppPermissionStatus.NotDetermined;
                case PhotoLibraryPermissionStatus.Restricted:
                    return HoloKitAppPermissionStatus.Denied;
                case PhotoLibraryPermissionStatus.Denied:
                    return HoloKitAppPermissionStatus.Denied;
                case PhotoLibraryPermissionStatus.Granted:
                    return HoloKitAppPermissionStatus.Granted;
                case PhotoLibraryPermissionStatus.Limited:
                    return HoloKitAppPermissionStatus.Granted;
                default:
                    return HoloKitAppPermissionStatus.Denied;
            }
        }

        public static HoloKitAppPermissionStatus GetLocationPermissionStatus()
        {
            LocationPermissionStatus status = PermissionsAPI.GetLocationPermissionStatus();
            switch (status)
            {
                case LocationPermissionStatus.NotDetermined:
                    return HoloKitAppPermissionStatus.NotDetermined;
                case LocationPermissionStatus.Restricted:
                    return HoloKitAppPermissionStatus.Denied;
                case LocationPermissionStatus.Denied:
                    return HoloKitAppPermissionStatus.Denied;
                case LocationPermissionStatus.AuthorizedAlways:
                    return HoloKitAppPermissionStatus.Granted;
                case LocationPermissionStatus.AuthorizedWhenInUse:
                    return HoloKitAppPermissionStatus.Granted;
                case LocationPermissionStatus.Authorized:
                    return HoloKitAppPermissionStatus.Granted;
                default:
                    return HoloKitAppPermissionStatus.Denied;
            }
        }
    }
}
