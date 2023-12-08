// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareQRCode";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RectTransform _background;

        [SerializeField] private RectTransform _playerListRoot;

        [SerializeField] private GameObject _playerSlotPrefab;

        [SerializeField] private RectTransform _qrCode;

        private const float DeviceSlotHeight = 50f;

        private const float DeviceListBottomPadding = 40f;

        private const float QRCodeWidth = 0.04f;

        private void Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                AdjustQRCodeSize();
                HoloKitApp.Instance.MultiplayerManager.StartAdvertising(CalculateCameraToQRCodeOffset());
            }
        }

        private void Update()
        {
            UpdateConnectedPlayerList();

            if (Screen.orientation != ScreenOrientation.Portrait)
                Screen.orientation = ScreenOrientation.Portrait;
        }

        private void AdjustQRCodeSize()
        {
            float qrCodeWithInPixel = HoloKitUtils.MeterToPixel(QRCodeWidth);
            _qrCode.sizeDelta = new Vector2(qrCodeWithInPixel, qrCodeWithInPixel);
        }

        private Vector3 CalculateCameraToQRCodeOffset()
        {
            // The offset from the left screen center to the QRCode
            Vector3 leftEdgeCenterToQRCodeOffset = Vector3.zero;
            leftEdgeCenterToQRCodeOffset.x = HoloKitUtils.PixelToMeter(Screen.width / 2f);
            leftEdgeCenterToQRCodeOffset.y = -HoloKitUtils.PixelToMeter(Screen.height / 2f - _qrCode.position.y);

            // The offset from the camera to the left screen center
            Vector3 originalPhoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 phoneModelCameraOffset = new(originalPhoneModelCameraOffset.y, -originalPhoneModelCameraOffset.x, originalPhoneModelCameraOffset.z);

            Vector3 cameraToQRCodeOffset = leftEdgeCenterToQRCodeOffset + phoneModelCameraOffset;
            return cameraToQRCodeOffset;
        }

        // TODO: Optimize this by using pooling
        private void UpdateConnectedPlayerList()
        {
            // Destroy previous slots
            foreach (Transform oldPlayer in _playerListRoot)
            {
                Destroy(oldPlayer.gameObject);
            }

            // Instantiate each player slot
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var playerSlot = Instantiate(_playerSlotPrefab, _playerListRoot);
                playerSlot.transform.localScale = Vector3.one;
                HoloKitAppPlayer player = client.PlayerObject.GetComponent<HoloKitAppPlayer>();
                string playerStatusString = client.ClientId == 0 ? "Host" : player.PlayerStatus.Value.ToString();
                playerSlot.GetComponent<TMP_Text>().text = $"{player.PlayerName.Value} ({playerStatusString})";
            }

            // Calculate the height
            int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
            float top = playerCount == 0 ? 0f : playerCount * DeviceSlotHeight + DeviceListBottomPadding;
            _background.offsetMax = new(0f, top);
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.MultiplayerManager.StopAdvertising();
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
