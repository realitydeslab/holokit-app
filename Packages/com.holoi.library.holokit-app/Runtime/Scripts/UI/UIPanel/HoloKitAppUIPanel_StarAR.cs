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

        [SerializeField] private GameObject _exitButton;

        [SerializeField] private GameObject _recordButton;

        private void Awake()
        {
            int a = Screen.width;
            int b = Screen.height;
            GetComponent<RectTransform>().sizeDelta = new Vector3(a > b ? a : b, a < b ? a : b);
        }

        public void OnTriggerButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _boostButton.SetActive(false);
            _exitButton.SetActive(false);
            _recordButton.SetActive(false);
        }

        public void OnTriggerButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
        }

        public void OnBoostButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _exitButton.SetActive(false);
            _recordButton.SetActive(false);
        }

        public void OnBoostButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
        }

        public void OnExitButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _recordButton.SetActive(false);
        }

        public void OnExitButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _recordButton.SetActive(true);
        }

        public void OnRecordButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _exitButton.SetActive(false);
        }

        public void OnRecordButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
        }
    }
}
