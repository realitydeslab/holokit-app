using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class ARPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _monoARPanel;

        [SerializeField] private GameObject _starARPanel;

        private void Start()
        {
            SwitchToMono();
        }

        public void SwitchToStAR()
        {
            _monoARPanel.SetActive(false);
            _starARPanel.SetActive(true);
            HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        public void SwitchToMono()
        {
            _monoARPanel.SetActive(true);
            _starARPanel.SetActive(false);
            HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Mono;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
