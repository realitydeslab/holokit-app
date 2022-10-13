using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_Product : MonoBehaviour
    {
        [SerializeField] private RectTransform _productImage;

        private const float StartPosY = -1767f;

        private const float EndPosY = -960f;

        private const float MovementSpeed = 410f;

        private void Start()
        {
            _productImage.anchoredPosition = new Vector2(_productImage.anchoredPosition.x, StartPosY);
        }

        private void Update()
        {
            if (_productImage.anchoredPosition.y < EndPosY)
            {
                _productImage.anchoredPosition += new Vector2(0f, MovementSpeed * Time.deltaTime);
            }
        }
    }
}
