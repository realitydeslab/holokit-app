using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;
using System;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUIEventsReactor : MonoBehaviour
    {
        public static event Action OnTriggered;

        private void Awake()
        {
            UI.PanelManager.OnRenderModeChanged += OnRenderModeChanged;
            UI.PanelManager.OnStartedSharingReality += OnStartedSharingReality;
            UI.PanelManager.OnStoppedSharingReality += OnStoppedSharingReality;
            UI.PanelManager.OnStARTriggered += OnTriggeredFunc;
        }

        private void OnDestroy()
        {
            
        }

        private void OnRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Stereo;
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnStartedSharingReality()
        {
            HoloKitApp.Instance.StartAdvertising();
        }

        private void OnStoppedSharingReality()
        {
            HoloKitApp.Instance.StopAdvertising();
        }

        private void OnTriggeredFunc()
        {
            OnTriggered?.Invoke();
        }

        private void OnBoostedFunc()
        {

        }

        private void OnExitReality()
        {

        }

        private void OnStartedRecording()
        {

        }

        private void OnStoppedRecording()
        {

        }


    }
}
