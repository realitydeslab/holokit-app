using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_Header : MonoBehaviour
    {
        private const float PhoneFrameTopOffset = 0.008f;

        private void Awake()
        {
            if (HoloKitHelper.IsRuntime)
            {
                if (HoloKitHelper.IsRuntime)
                {
                    Vector2 phoneFrameSize = HoloKitOpticsAPI.GetHoloKitModelPhoneFrameSize(HoloKitType.HoloKitX);
                    float phoneFramePixelHeight = HoloKitAppUtils.MeterToPixel(phoneFrameSize.y);

                    float posY = phoneFramePixelHeight + HoloKitAppUtils.MeterToPixel(PhoneFrameTopOffset);
                    Debug.Log($"Header posY: {posY}");
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posY);
                }
            }
        }
    }
}
