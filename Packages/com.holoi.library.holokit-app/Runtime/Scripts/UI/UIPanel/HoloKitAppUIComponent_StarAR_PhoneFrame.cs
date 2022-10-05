using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_PhoneFrame : MonoBehaviour
    {
        private void Awake()
        {
            if (HoloKitHelper.IsRuntime)
            {
                Vector2 phoneFrameSize = HoloKitOpticsAPI.GetHoloKitModelPhoneFrameSize(HoloKitType.HoloKitX);
                int phoneFramePixelWidth = HoloKitAppUtils.MeterToPixel(phoneFrameSize.x);
                int phoneFramePixelHeight = HoloKitAppUtils.MeterToPixel(phoneFrameSize.y + 0.005f); // Add an extra height
                Debug.Log($"Phone frame width: {phoneFramePixelWidth} and height: {phoneFramePixelHeight}");
                GetComponent<RectTransform>().sizeDelta = new Vector2(phoneFramePixelWidth, phoneFramePixelHeight);
            }
        }
    }
}
