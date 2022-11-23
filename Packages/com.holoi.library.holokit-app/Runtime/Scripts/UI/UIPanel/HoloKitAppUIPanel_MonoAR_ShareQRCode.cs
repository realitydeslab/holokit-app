using System.Collections.Generic;
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

        [SerializeField] private Transform _qrCode;

        private const float BackgroundDefaultHeight = 1162f;

        private const float DeviceSlotHeight = 50f;

        private const float DeviceListBottomPadding = 40f;

        private const float QRCodeWidth = 0.04f;

        private void Start()
        {
            HoloKitAppMultiplayerManager.OnConnectedDeviceListUpdated += OnConnectedDeviceListUpdated;
            OnConnectedDeviceListUpdated(HoloKitApp.Instance.MultiplayerManager.ConnectedDevicesList);
            AdjustQRCodeSize();
            if (HoloKitUtils.IsRuntime)
            {
                CalculateCameraToQRCodeOffset();
                HoloKitAppUIEventManager.OnStartedAdvertising?.Invoke();
            }
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnConnectedDeviceListUpdated += OnConnectedDeviceListUpdated;
        }

        private void AdjustQRCodeSize()
        {
            float qrCodeWithInPixel = HoloKitUtils.MeterToPixel(QRCodeWidth);
            GetComponent<RectTransform>().sizeDelta = new Vector2(qrCodeWithInPixel, qrCodeWithInPixel);
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
            HoloKitApp.Instance.MultiplayerManager.HostCameraToQRCodeOffset = cameraToQRCodeOffset;
        }

        private void OnConnectedDeviceListUpdated(List<DeviceInfo> deviceInfos)
        {
            // Destroy previous slots
            for (int i = 0; i < _deviceListRoot.childCount; i++)
            {
                Destroy(_deviceListRoot.GetChild(i).gameObject);
            }
            int deviceCount = 0;
            foreach (var deviceInfo in deviceInfos)
            {
                var deviceSlot = Instantiate(_deviceSlotPrefab, _deviceListRoot);
                deviceSlot.transform.localScale = Vector3.one;
                deviceSlot.GetComponent<TMP_Text>().text = $"{deviceInfo.Name} ({deviceInfo.Status})";
                deviceCount++;
            }
            if (deviceCount == 0)
            {
                _background.sizeDelta = new Vector2(_background.sizeDelta.x, BackgroundDefaultHeight);
            }
            else
            {
                _background.sizeDelta = new Vector2(_background.sizeDelta.x,
                    BackgroundDefaultHeight + deviceCount * DeviceSlotHeight + DeviceListBottomPadding);
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIEventManager.OnStoppedAdvertising?.Invoke();
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
