using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUIEventsReactor : MonoBehaviour
    {
        private void Awake()
        {
            
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
    }
}
