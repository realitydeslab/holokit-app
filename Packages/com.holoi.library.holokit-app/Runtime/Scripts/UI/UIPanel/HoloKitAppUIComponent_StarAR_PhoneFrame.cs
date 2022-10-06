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
                float phoneFramePixelWidth = HoloKitAppUtils.MeterToPixel(phoneFrameSize.x);
                float phoneFramePixelHeight = HoloKitAppUtils.MeterToPixel(phoneFrameSize.y);
                Debug.Log($"Phone frame width: {phoneFramePixelWidth} and height: {phoneFramePixelHeight}");
                GetComponent<RectTransform>().sizeDelta = new Vector2(phoneFramePixelWidth, phoneFramePixelHeight);
            }
        }
    }
}
