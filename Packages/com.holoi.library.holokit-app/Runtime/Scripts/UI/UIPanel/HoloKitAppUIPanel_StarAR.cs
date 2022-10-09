using System.Collections;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_StarAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "StarAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _horizontalAlignmentMarker;

        [SerializeField] private GameObject _mainWindow;

        [SerializeField] private GameObject _moreWindow;

        [SerializeField] private GameObject _triggerButton;

        [SerializeField] private GameObject _boostButton;

        [SerializeField] private GameObject _exitButton;

        [SerializeField] private GameObject _recordButton;

        [SerializeField] private GameObject _moreButton;

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
            _moreButton.SetActive(false);
        }

        public void OnTriggerButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnBoostButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _exitButton.SetActive(false);
            _recordButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnBoostButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnExitButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _recordButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnExitButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnRecordButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _exitButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnRecordButtonReleased()
        {
            _horizontalAlignmentMarker.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnMoreButtonPressed()
        {
            _horizontalAlignmentMarker.SetActive(false);
            _mainWindow.SetActive(false);
            _moreWindow.SetActive(true);
            StartCoroutine(StartMoreButtonCoolingDown());
        }

        private IEnumerator StartMoreButtonCoolingDown()
        {
            yield return new WaitForSeconds(6f);
            _horizontalAlignmentMarker.SetActive(true);
            _mainWindow.SetActive(true);
            _moreWindow.SetActive(false);
        }
    }
}
