using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_TriggerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float HorizontalOffset = 0.053f;

        private void Awake()
        {
            float horizontalOffsetPixel = HoloKitAppUtils.MeterToPixel(HorizontalOffset);
            Debug.Log($"Trigger button offset: {horizontalOffsetPixel}");
            GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffsetPixel, 0f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //_starARPanel.OnBoostButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //_starARPanel.OnBoostButtonReleased();
        }
    }
}
