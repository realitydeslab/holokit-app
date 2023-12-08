// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_StarAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "StarAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RectTransform _phoneFrame;

        [SerializeField] private RectTransform _horizontalAlignmentMarker;

        [SerializeField] private RectTransform _header;

        [SerializeField] private GameObject _mainWindow;

        [SerializeField] private GameObject _moreWindow;

        [SerializeField] private GameObject _triggerButton;

        [SerializeField] private GameObject _boostButton;

        [SerializeField] private GameObject _exitButton;

        [SerializeField] private GameObject _recordButton;

        [SerializeField] private GameObject _moreButton;

        [SerializeField] private GameObject _block;

        // TODO: Make this dynamically adapted to the current iPhone
        private const float HorizontalAlignmentMarkerThickness = 3f;

        // In meter
        private const float PhoneFrameTopToHeaderOffset = 0.005f;

        private void Start()
        {
            // Adapt to the current screen size
            int a = Screen.width;
            int b = Screen.height;
            int screenWidth = a > b ? a : b;
            int screenHeight = a < b ? a : b;
            GetComponent<RectTransform>().sizeDelta = new Vector3(screenWidth, screenHeight);

            if (HoloKitUtils.IsEditor) { return; }
            // Set phone frame
            Vector2 phoneFrameSizeInPixel = HoloKitOpticsAPI.GetHoloKitModelPhoneFrameSizeInPixel();
            _phoneFrame.sizeDelta = phoneFrameSizeInPixel;

            // Set horizontal alignment marker
            // Adjust position
            float horizontalAlignmentMarkerOffsetInPixel = HoloKitOpticsAPI.GetHoloKitModelHorizontalAlignmentMarkerOffsetInPixel(HoloKitType.HoloKitX);
            _horizontalAlignmentMarker.anchoredPosition = new Vector2(horizontalAlignmentMarkerOffsetInPixel, 0f);
            // Adjust length
            _horizontalAlignmentMarker.sizeDelta = new Vector2(HorizontalAlignmentMarkerThickness, screenHeight - phoneFrameSizeInPixel.y);

            // Set header
            _header.anchoredPosition = new Vector2(0f, phoneFrameSizeInPixel.y + HoloKitUtils.MeterToPixel(PhoneFrameTopToHeaderOffset));
        }

        public void OnTriggerButtonPressed()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(false);
            _boostButton.SetActive(false);
            _exitButton.SetActive(false);
            _recordButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnTriggerButtonReleased()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnBoostButtonPressed()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(false);
            _triggerButton.SetActive(false);
            _exitButton.SetActive(false);
            _recordButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnBoostButtonReleased()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(true);
            _triggerButton.SetActive(true);
            _exitButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnExitButtonPressed()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _recordButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnExitButtonReleased()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _recordButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnRecordButtonPressed()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(false);
            _triggerButton.SetActive(false);
            _boostButton.SetActive(false);
            _exitButton.SetActive(false);
            _moreButton.SetActive(false);
        }

        public void OnRecordButtonReleased()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(true);
            _triggerButton.SetActive(true);
            _boostButton.SetActive(true);
            _exitButton.SetActive(true);
            _moreButton.SetActive(true);
        }

        public void OnMoreButtonPressed()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(false);
            //_mainWindow.SetActive(false);
            _block.SetActive(true);
            _moreWindow.SetActive(true);
        }

        public void OnMoreButtonReleased()
        {
            _horizontalAlignmentMarker.gameObject.SetActive(true);
            //_mainWindow.SetActive(true);
            _block.SetActive(false);
            _moreWindow.SetActive(false);
        }
    }
}
