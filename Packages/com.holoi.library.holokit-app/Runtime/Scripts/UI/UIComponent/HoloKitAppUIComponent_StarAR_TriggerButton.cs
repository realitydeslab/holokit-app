// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_TriggerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject _untriggered;

        [SerializeField] private GameObject _triggered;

        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        private const float HorizontalOffset = 0.053f;

        private void Awake()
        {
            //float horizontalOffsetPixel = HoloKitAppUtils.MeterToPixel(HorizontalOffset);
            //GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffsetPixel, 0f);

            _untriggered.SetActive(true);
            _triggered.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            _untriggered.SetActive(false);
            _triggered.SetActive(true);
            _starARPanel.OnTriggerButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _untriggered.SetActive(true);
            _triggered.SetActive(false);
            _starARPanel.OnTriggerButtonReleased();
        }
    }
}
