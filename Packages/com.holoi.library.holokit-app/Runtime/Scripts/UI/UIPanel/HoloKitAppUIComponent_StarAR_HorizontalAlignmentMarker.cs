using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_HorizontalAlignmentMarker : MonoBehaviour
    {
        private void Awake()
        {
            if (HoloKitHelper.IsRuntime)
            {
                var rectTransform = GetComponent<RectTransform>();
                // Adjust marker position
                float horizontalAlignmentMarkerOffset = HoloKitOpticsAPI.GetHoloKitModelHorizontalAlignmentMarkerOffset(HoloKitType.HoloKitX);
                rectTransform.anchoredPosition = new Vector2(HoloKitAppUtils.MeterToPixel(horizontalAlignmentMarkerOffset), 0f);
                Debug.Log($"Hozirontal alignmenr marker offset in pixel: {HoloKitAppUtils.MeterToPixel(horizontalAlignmentMarkerOffset)}");

                // Adjust marker length
                Vector2 phoneFrameSize = HoloKitOpticsAPI.GetHoloKitModelPhoneFrameSize(HoloKitType.HoloKitX);
                int phoneFramePixelHeight = HoloKitAppUtils.MeterToPixel(phoneFrameSize.y + 0.005f);
                float screenHeight = Screen.width > Screen.height ? Screen.height : Screen.width;
                rectTransform.sizeDelta = new Vector2(3, screenHeight - phoneFramePixelHeight);

                // TODO: Adjust marker thickness
            }
        }
    }
}
