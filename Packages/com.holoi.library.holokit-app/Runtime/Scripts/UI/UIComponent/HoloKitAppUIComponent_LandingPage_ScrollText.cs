using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_ScrollText : MonoBehaviour
    {
        [SerializeField] private RectTransform _text1;

        [SerializeField] private RectTransform _text2;

        private const float MovementSpeed = 80f;

        private void Update()
        {
            _text1.anchoredPosition -= new Vector2(MovementSpeed * Time.deltaTime, 0f);
            _text2.anchoredPosition -= new Vector2(MovementSpeed * Time.deltaTime, 0f);

            if (_text1.anchoredPosition.x < -_text1.sizeDelta.x)
            {
                _text1.anchoredPosition = new Vector2(_text1.sizeDelta.x, 0f);
            }
            else if (_text2.anchoredPosition.x < -_text1.sizeDelta.x)
            {
                _text2.anchoredPosition = new Vector2(_text1.sizeDelta.x, 0f);
            }
        }
    }
}
