using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Holoi.Library.Permissions;
using HoloKit;

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
        public static bool MandatoryPermissionsGranted()
        {
            if (HoloKitHelper.IsEditor)
            {
                return false;
            }

            if (GetCameraPermissionStatus() != HoloKitAppPermissionStatus.Granted)
            {
                return false;
            }
            if (GetMicrophonePermissionStatus() != HoloKitAppPermissionStatus.Granted)
            {
                return false;
            }
            if (GetPhotoLibraryAddPermissionStatus() != HoloKitAppPermissionStatus.Granted)
            {
                return false;
            }
            if (GetLocationPermissionStatus() != HoloKitAppPermissionStatus.Granted)
            {
                return false;
            }
            return true;
        }

        public static HoloKitAppPermissionStatus GetCameraPermissionStatus()
        {
            if (HoloKitHelper.IsEditor)
            {
                return HoloKitAppPermissionStatus.Granted;
            }

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
            if (HoloKitHelper.IsEditor)
            {
                return HoloKitAppPermissionStatus.Granted;
            }

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
            if (HoloKitHelper.IsEditor)
            {
                return HoloKitAppPermissionStatus.Granted;
            }

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
            if (HoloKitHelper.IsEditor)
            {
                return HoloKitAppPermissionStatus.Granted;
            }

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

        public static IEnumerator RequestWirelessDataPermission()
        {
            yield return new WaitForSeconds(0.2f); // TODO: To make sure the UI has already popped up
            UnityWebRequest request = UnityWebRequest.Get("http://holoi.com");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("[HoloKitAppPermission] Error While Sending: " + request.error);
            }
            else
            {
                //Debug.Log("[HoloKitAppPermission] Received: " + request.downloadHandler.text);
            }
        }
    }
}
