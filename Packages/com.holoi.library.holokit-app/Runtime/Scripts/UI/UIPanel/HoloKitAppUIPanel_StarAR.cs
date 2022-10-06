using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_StarAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "StarAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _horizontalAlignmentMarker;

        [SerializeField] private GameObject _triggerButton;

        [SerializeField] private GameObject _boostButton;

        private void Awake()
        {
            int a = Screen.width;
            int b = Screen.height;
            GetComponent<RectTransform>().sizeDelta = new Vector3(a > b ? a : b, a < b ? a : b);
        }

        public void OnTriggerButtonPressed()
        {

        }

        public void OnTriggerButtonReleased()
        {

        }

        public void OnBoostButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
        }

        public void OnBoostButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
        }
    }
}
