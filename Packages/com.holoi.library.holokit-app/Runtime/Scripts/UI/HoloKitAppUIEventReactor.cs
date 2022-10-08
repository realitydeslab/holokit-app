using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIEventReactor : MonoBehaviour
    {
        private void Awake()
        {
            HoloKitAppUIEventManager.OnRenderModeChanged += OnRenderModeChanged;
            HoloKitAppUIEventManager.OnStartedAdvertising += OnStartedSharingReality;
            HoloKitAppUIEventManager.OnStoppedAdvertising += OnStoppedSharingReality;
            HoloKitAppUIEventManager.OnStartedRecording += OnStartedRecording;
            HoloKitAppUIEventManager.OnStoppedRecording += OnStoppedRecording;
            HoloKitAppUIEventManager.OnExit += OnExitReality;
            HoloKitAppUIEventManager.OnAlignmentMarkChecked += OnAlignmentMarkChecked;
            HoloKitAppUIEventManager.OnRescanQRCode += OnRescanQRCode;
        }

        private void OnDestroy()
        {
            HoloKitAppUIEventManager.OnRenderModeChanged -= OnRenderModeChanged;
            HoloKitAppUIEventManager.OnStartedAdvertising -= OnStartedSharingReality;
            HoloKitAppUIEventManager.OnStoppedAdvertising -= OnStoppedSharingReality;
            HoloKitAppUIEventManager.OnStartedRecording -= OnStartedRecording;
            HoloKitAppUIEventManager.OnStoppedRecording -= OnStoppedRecording;
            HoloKitAppUIEventManager.OnExit -= OnExitReality;
            HoloKitAppUIEventManager.OnAlignmentMarkChecked -= OnAlignmentMarkChecked;
            HoloKitAppUIEventManager.OnRescanQRCode -= OnRescanQRCode;
        }

        private void OnRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
                //Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Mono;
                //Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnStartedSharingReality()
        {
            HoloKitApp.Instance.MultiplayerManager.StartAdvertising();
        }

        private void OnStoppedSharingReality()
        {
            HoloKitApp.Instance.MultiplayerManager.StopAdvertising();
        }

        private void OnExitReality()
        {
            HoloKitApp.Instance.Shutdown();
        }

        private void OnStartedRecording()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitApp.Instance.Recorder.StartRecording();
            }
            else
            {
                Debug.Log("[HoloKitApp] You can only record video on iOS devices");
            }
        }

        private void OnStoppedRecording()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitApp.Instance.Recorder.StopRecording();
            }
            else
            {
                Debug.Log("[HoloKitApp] You can only record video on iOS devices");
            }
        }

        private void OnAlignmentMarkChecked()
        {
            HoloKitApp.Instance.MultiplayerManager.CheckAlignmentMark();
        }

        private void OnRescanQRCode()
        {
            HoloKitApp.Instance.MultiplayerManager.RescanQRCode();
        }
    }
}
