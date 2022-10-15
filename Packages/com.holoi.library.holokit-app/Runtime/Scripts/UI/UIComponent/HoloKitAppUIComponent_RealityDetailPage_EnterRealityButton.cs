using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_EnterRealityButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _enterRingImage;

        [SerializeField] private Image _playImage;

        [SerializeField] private Color _normalColor;

        [SerializeField] private Color _pressedColor;

        private const float RotationSpeed = 30f;

        private void Update()
        {
            _enterRingImage.transform.Rotate(new Vector3(0f, 0f, 1f), RotationSpeed * Time.deltaTime);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _enterRingImage.color = _pressedColor;
            _playImage.color = _pressedColor;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _enterRingImage.color = _normalColor;
            _playImage.color = _normalColor;
        }

        public void OnEnterRealityButtonPressed()
        {
            Debug.Log("Enter reality button pressed");
        }
    }
}
