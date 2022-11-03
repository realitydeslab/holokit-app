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
            HoloKitAppUIEventManager.OnExitReality += OnExitReality;
            HoloKitAppUIEventManager.OnAlignmentMarkChecked += OnAlignmentMarkChecked;
            HoloKitAppUIEventManager.OnRescanQRCode += OnRescanQRCode;
            HoloKitAppUIEventManager.OnExitNoLiDARScene += OnExitNoLiDARScene;
        }

        private void OnDestroy()
        {
            HoloKitAppUIEventManager.OnRenderModeChanged -= OnRenderModeChanged;
            HoloKitAppUIEventManager.OnStartedAdvertising -= OnStartedSharingReality;
            HoloKitAppUIEventManager.OnStoppedAdvertising -= OnStoppedSharingReality;
            HoloKitAppUIEventManager.OnStartedRecording -= OnStartedRecording;
            HoloKitAppUIEventManager.OnStoppedRecording -= OnStoppedRecording;
            HoloKitAppUIEventManager.OnExitReality -= OnExitReality;
            HoloKitAppUIEventManager.OnAlignmentMarkChecked -= OnAlignmentMarkChecked;
            HoloKitAppUIEventManager.OnRescanQRCode -= OnRescanQRCode;
            HoloKitAppUIEventManager.OnExitNoLiDARScene -= OnExitNoLiDARScene;
        }

        private void OnRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
            }
            else
            {
                HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Mono;
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
            if (HoloKitUtils.IsRuntime)
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
            if (HoloKitUtils.IsRuntime)
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

        private void OnExitNoLiDARScene()
        {
            HoloKitApp.Instance.ExitNoLiDARScene();
        }
    }
}
