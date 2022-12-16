using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareQRCode";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RectTransform _background;

        [SerializeField] private RectTransform _deviceListRoot;

        [SerializeField] private GameObject _deviceSlotPrefab;

        [SerializeField] private RectTransform _qrCode;

        private const float BackgroundDefaultHeight = 1162f;

        private const float DeviceSlotHeight = 50f;

        private const float DeviceListBottomPadding = 40f;

        private const float QRCodeWidth = 0.04f;

        private void Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                AdjustQRCodeSize();
                CalculateCameraToQRCodeOffset();
            }
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated += OnConnectedPlayerListUpdated;
            OnConnectedPlayerListUpdated();
            StartCoroutine(HoloKitAppUtils.WaitAndDo(1f, () =>
            {
                HoloKitApp.Instance.MultiplayerManager.StartAdvertising();
            }));
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated -= OnConnectedPlayerListUpdated;
        }

        private void AdjustQRCodeSize()
        {
            float qrCodeWithInPixel = HoloKitUtils.MeterToPixel(QRCodeWidth);
            _qrCode.sizeDelta = new Vector2(qrCodeWithInPixel, qrCodeWithInPixel);
        }

        private void CalculateCameraToQRCodeOffset()
        {
            // The offset from the left screen center to the QRCode
            Vector3 leftEdgeCenterToQRCodeOffset = Vector3.zero;
            leftEdgeCenterToQRCodeOffset.x = HoloKitUtils.PixelToMeter(Screen.width / 2f);
            leftEdgeCenterToQRCodeOffset.y = -HoloKitUtils.PixelToMeter(Screen.height / 2f - _qrCode.position.y);

            // The offset from the camera to the left screen center
            Vector3 originalPhoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 phoneModelCameraOffset = new(originalPhoneModelCameraOffset.y, -originalPhoneModelCameraOffset.x, originalPhoneModelCameraOffset.z);

            Vector3 cameraToQRCodeOffset = leftEdgeCenterToQRCodeOffset + phoneModelCameraOffset;
            HoloKitApp.Instance.MultiplayerManager.HostCameraToQRCodeOffset.Value = cameraToQRCodeOffset;
        }

        private void OnConnectedPlayerListUpdated()
        {
            // Destroy previous slots
            foreach (Transform child in _deviceListRoot)
            {
                Destroy(child.gameObject);
            }
            int playerCount = 0;
            var playerList = HoloKitApp.Instance.MultiplayerManager.ConnectedPlayerList;
            var masterPlayer = HoloKitApp.Instance.MultiplayerManager.GetMasterPlayer();
            foreach (var player in playerList)
            {
                var deviceSlot = Instantiate(_deviceSlotPrefab, _deviceListRoot);
                deviceSlot.transform.localScale = Vector3.one;
                string statusStr = player.OwnerClientId == masterPlayer.OwnerClientId ? "Host" : player.Status.ToString();
                deviceSlot.GetComponent<TMP_Text>().text = $"{player.Name} ({statusStr})";
                playerCount++;
            }
            if (playerCount == 0)
            {
                _background.sizeDelta = new Vector2(_background.sizeDelta.x, BackgroundDefaultHeight);
            }
            else
            {
                _background.sizeDelta = new Vector2(_background.sizeDelta.x,
                    BackgroundDefaultHeight + playerCount * DeviceSlotHeight + DeviceListBottomPadding);
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.MultiplayerManager.StopAdvertising();
        }
    }
}
