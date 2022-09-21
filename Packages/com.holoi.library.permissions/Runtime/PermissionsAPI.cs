using System;
using System.Runtime.InteropServices;

namespace Holoi.Library.Permissions
{
    public enum CameraPermissionStatus
    {
        NotDetermined = 0,
        Restricted = 1,
        Denied = 2,
        Granted = 3,
    }

    public enum MicrophonePermissionStatus
    {
        NotDetermined = 0,
        Denied = 1,
        Granted = 2
    }

    public enum PhotoLibraryPermissionStatus
    {
        NotDetermined = 0,
        Restricted = 1,
        Denied = 2,
        Granted = 3,
        Limited = 4
    }

    public enum LocationPermissionStatus
    {
        NotDetermined = 0,
        Restricted = 1,
        Denied = 2,
        AuthorizedAlways = 3,
        AuthorizedWhenInUse = 4,
        Authorized = 5
    }

    public static class PermissionsAPI
    {
        [DllImport("__Internal")]
        private static extern void Permissions_Initialize(Action<bool> OnRequestCameraPermissionCompleted,
                                                          Action<bool> OnRequestMicrophonePermissionCompleted,
                                                          Action<int> OnRequestPhotoLibraryAddPermissionCompleted,
                                                          Action<int> OnLocationPermissionStatusChanged);

        [DllImport("__Internal")]
        private static extern int Permissions_GetCameraPermissionStatus();

        [DllImport("__Internal")]
        private static extern void Permissions_RequestCameraPermission();

        [DllImport("__Internal")]
        private static extern int Permissions_GetMicrophonePermissionStatus();

        [DllImport("__Internal")]
        private static extern void Permissions_RequestMicrophonePermission();

        [DllImport("__Internal")]
        private static extern int Permissions_GetPhotoLibraryAddPermissionStatus();

        [DllImport("__Internal")]
        private static extern void Permissions_RequestPhotoLibraryAddPermission();

        [DllImport("__Internal")]
        private static extern int Permissions_GetLocationPermissionStatus();

        [DllImport("__Internal")]
        private static extern void Permissions_RequestLocationWhenInUsePermission();

        [DllImport("__Internal")]
        private static extern void Permissions_RequestLocationAlwaysPermission();

        [DllImport("__Internal")]
        private static extern void Permissions_OpenAppSettings();

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void OnRequestCameraPermissionCompletedDelegate(bool granted)
        {
            OnRequestCameraPermissionCompleted?.Invoke(granted);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void OnRequestMicrophonePermissionCompletedDelegate(bool granted)
        {
            OnRequestMicrophonePermissionCompleted?.Invoke(granted);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnRequestPhotoLibraryAddPermissionCompletedDelegate(int status)
        {
            OnRequestPhotoLibraryAddPermissionCompleted?.Invoke((PhotoLibraryPermissionStatus)status);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnLocationPermissionStatusChangedDelegate(int status)
        {
            OnLocationPermissionStatusChanged?.Invoke((LocationPermissionStatus)status);
        }

        public static event Action<bool> OnRequestCameraPermissionCompleted;

        public static event Action<bool> OnRequestMicrophonePermissionCompleted;

        public static event Action<PhotoLibraryPermissionStatus> OnRequestPhotoLibraryAddPermissionCompleted;

        public static event Action<LocationPermissionStatus> OnLocationPermissionStatusChanged;

        public static void Initialize()
        {
            Permissions_Initialize(OnRequestCameraPermissionCompletedDelegate,
                                   OnRequestMicrophonePermissionCompletedDelegate,
                                   OnRequestPhotoLibraryAddPermissionCompletedDelegate,
                                   OnLocationPermissionStatusChangedDelegate);
        }

        public static CameraPermissionStatus GetCameraPermissionStatus()
        {
            return (CameraPermissionStatus)Permissions_GetCameraPermissionStatus();
        }

        public static void RequestCameraPermission()
        {
            Permissions_RequestCameraPermission();
        }

        public static MicrophonePermissionStatus GetMicrophonePermissionStatus()
        {
            return (MicrophonePermissionStatus)Permissions_GetMicrophonePermissionStatus();
        }

        public static void RequestMicrophonePermission()
        {
            Permissions_RequestMicrophonePermission();
        }

        public static PhotoLibraryPermissionStatus GetPhotoLibraryAddPermissionStatus()
        {
            return (PhotoLibraryPermissionStatus)Permissions_GetPhotoLibraryAddPermissionStatus();
        }

        public static void RequestPhotoLibraryAddPermission()
        {
            Permissions_RequestPhotoLibraryAddPermission();
        }

        public static LocationPermissionStatus GetLocationPermissionStatus()
        {
            return (LocationPermissionStatus)Permissions_GetLocationPermissionStatus();
        }

        public static void RequestLocationWhenInUsePermission()
        {
            Permissions_RequestLocationWhenInUsePermission();
        }

        public static void RequestLocationAlwaysPermission()
        {
            Permissions_RequestLocationAlwaysPermission();
        }

        public static void OpenAppSettings()
        {
            Permissions_OpenAppSettings();
        }
    }
}