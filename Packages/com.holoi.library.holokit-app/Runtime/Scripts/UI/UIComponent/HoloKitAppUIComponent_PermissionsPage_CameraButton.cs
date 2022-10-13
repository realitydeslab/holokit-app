using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp.UI
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
            return HoloKitAppPermissionsManager.GetMicrophonePermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestMicrophonePermission();
        }
    }
}
