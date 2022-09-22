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

        public static event Action OnBoosted;

        private void Awake()
        {
            UI.PanelManager.OnRenderModeChanged += OnRenderModeChanged;
            UI.PanelManager.OnStartedSharingReality += OnStartedSharingReality;
            UI.PanelManager.OnStoppedSharingReality += OnStoppedSharingReality;
            UI.PanelManager.OnStARTriggered += OnTriggeredFunc;
            UI.PanelManager.OnStARBoosted += OnBoostedFunc;
            UI.PanelManager.OnStartedRecording += OnStartedRecording;
            UI.PanelManager.OnStoppedRecording += OnStoppedRecording;
            UI.PanelManager.OnExitReality += OnExitReality;
        }

        private void OnDestroy()
        {
            UI.PanelManager.OnRenderModeChanged -= OnRenderModeChanged;
            UI.PanelManager.OnStartedSharingReality -= OnStartedSharingReality;
            UI.PanelManager.OnStoppedSharingReality -= OnStoppedSharingReality;
            UI.PanelManager.OnStARTriggered -= OnTriggeredFunc;
            UI.PanelManager.OnStARBoosted -= OnBoostedFunc;
            UI.PanelManager.OnStartedRecording -= OnStartedRecording;
            UI.PanelManager.OnStoppedRecording -= OnStoppedRecording;
            UI.PanelManager.OnExitReality -= OnExitReality;
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
            HoloKitApp.Instance.RealityManager.StartAdvertising();
        }

        private void OnStoppedSharingReality()
        {
            HoloKitApp.Instance.RealityManager.StopAdvertising();
        }

        private void OnTriggeredFunc()
        {
            Debug.Log("[HoloKitAppUIEventsReactor] OnTriggeredFunc");
            OnTriggered?.Invoke();
        }

        private void OnBoostedFunc()
        {
            OnBoosted?.Invoke();
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