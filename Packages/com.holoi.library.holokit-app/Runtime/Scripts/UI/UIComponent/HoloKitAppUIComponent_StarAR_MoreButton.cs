using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_MoreButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        public void OnPointerDown(PointerEventData eventData)
        {
            _starARPanel.OnMoreButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}
