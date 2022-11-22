using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareQRCode";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RectTransform _background;

        [SerializeField] private RectTransform _deviceListRoot;

        [SerializeField] private GameObject _deviceSlotPrefab;

        private const float BackgroundDefaultHeight = 1162f;

        private const float DeviceSlotHeight = 50f;

        private const float DeviceListBottomPadding = 40f;

        private void Start()
        {
            HoloKitAppMultiplayerManager.OnConnectedDeviceListUpdated += OnConnectedDeviceListUpdated;
            OnConnectedDeviceListUpdated(HoloKitApp.Instance.MultiplayerManager.ConnectedDevicesList);
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnConnectedDeviceListUpdated += OnConnectedDeviceListUpdated;
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
