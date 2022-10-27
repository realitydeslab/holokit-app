using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_Product : MonoBehaviour
    {
        [SerializeField] private RectTransform _productImage;

        private const float StartPosY = -1767f;

        private const float EndPosY = -960f;

        private const float MovementDuration = 1.2f;

        private float _movementSpeed;

        private void Start()
        {
            _productImage.anchoredPosition = new Vector2(_productImage.anchoredPosition.x, StartPosY);
            _movementSpeed = (EndPosY - StartPosY) / MovementDuration;
        }

        private void Update()
        {
            if (_productImage.anchoredPosition.y < EndPosY)
            {
                _productImage.anchoredPosition += new Vector2(0f, _movementSpeed * Time.deltaTime);
            }
        }
    }
}
