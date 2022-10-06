using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_BoostButton : HoloKitAppUITemplate_HorizontalScrollButton
    {
        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        private const float HorizontalOffset = 0.018f;

        protected override void Awake()
        {
            base.Awake();

            float horizontalOffsetPixel = HoloKitAppUtils.MeterToPixel(HorizontalOffset);
            Debug.Log($"Boost button offset: {horizontalOffsetPixel}");
            GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffsetPixel, 0f);
        }

        protected override void OnSelected()
        {
            base.OnSelected();

            _starARPanel.OnBoostButtonPressed();
        }

        protected override void OnRecovered()
        {
            base.OnRecovered();

            _starARPanel.OnBoostButtonReleased();
        }
    }
}
