using System.Collections;
using System.Collections.Generic;
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
            float horizontalOffsetPixel = HoloKitAppUtils.MeterToPixel(HorizontalOffset);
            Debug.Log($"Trigger button offset: {horizontalOffsetPixel}");
            GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffsetPixel, 0f);

            _untriggered.SetActive(true);
            _triggered.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HoloKitAppUIEventManager.OnTriggered?.Invoke();
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
