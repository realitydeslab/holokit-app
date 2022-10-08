using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_QRCode : MonoBehaviour
    {
        private void Start()
        {
            if (HoloKitHelper.IsEditor) { return; }

            // Calculate correct camera to QR code offset
            Vector3 leftEdgeCenterToQRCodeOffset = Vector3.zero;
            leftEdgeCenterToQRCodeOffset.x = HoloKitAppUtils.PixelToMeter(Screen.width / 2);
            leftEdgeCenterToQRCodeOffset.y = -HoloKitAppUtils.PixelToMeter(Screen.height / 2 - (int)transform.position.y);

            Vector3 originalPhoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 phoneModelCameraOffset = new(originalPhoneModelCameraOffset.y, -originalPhoneModelCameraOffset.x, originalPhoneModelCameraOffset.z);

            Vector3 cameraToQRCodeOffset = leftEdgeCenterToQRCodeOffset + phoneModelCameraOffset;
            Debug.Log($"CameraToQRCodeOffset: {cameraToQRCodeOffset:F4}");
            HoloKitApp.Instance.MultiplayerManager.CameraToQRCodeOffset = cameraToQRCodeOffset;
        }
    }
}
