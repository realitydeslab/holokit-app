using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_MoreButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("More button OnPointerDown");
            _starARPanel.OnMoreButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("More button OnPointerUp");
            _starARPanel.OnMoreButtonReleased();
        }
    }
}
