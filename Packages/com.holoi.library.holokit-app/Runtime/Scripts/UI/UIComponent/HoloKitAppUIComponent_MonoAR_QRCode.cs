using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_QRCode : MonoBehaviour
    {
        private const float QrCodeWidth = 0.04f;

        private void Start()
        {
            // Adjust QRCode size
            float qrCodeWithInPixel = HoloKitUtils.MeterToPixel(QrCodeWidth);
            GetComponent<RectTransform>().sizeDelta = new Vector2(qrCodeWithInPixel, qrCodeWithInPixel);

            if (HoloKitUtils.IsEditor) { return; }

            // Calculate correct camera to QR code offset
            Vector3 leftEdgeCenterToQRCodeOffset = Vector3.zero;
            leftEdgeCenterToQRCodeOffset.x = HoloKitUtils.PixelToMeter(Screen.width / 2);
            leftEdgeCenterToQRCodeOffset.y = -HoloKitUtils.PixelToMeter(Screen.height / 2 - (int)transform.position.y);

            Vector3 originalPhoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 phoneModelCameraOffset = new(originalPhoneModelCameraOffset.y, -originalPhoneModelCameraOffset.x, originalPhoneModelCameraOffset.z);

            Vector3 cameraToQRCodeOffset = leftEdgeCenterToQRCodeOffset + phoneModelCameraOffset;
            HoloKitApp.Instance.MultiplayerManager.CameraToQRCodeOffset = cameraToQRCodeOffset;
        }
    }
}
