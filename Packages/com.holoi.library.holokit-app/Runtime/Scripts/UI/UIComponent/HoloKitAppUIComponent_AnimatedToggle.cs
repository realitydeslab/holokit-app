using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_AnimatedToggle : MonoBehaviour
    {
        [SerializeField] private Image _frameImage;

        [SerializeField] private Image _selectorImage;

        public bool Toggled;

        private const float MaxAbsSelectorPosX = 40f;

        private const float SelectorMovementDuration = 0.18f;

        private float _selectorMovementSpeed;

        private readonly Color WhiteColor = new(1f, 1f, 1f, 1f);

        private readonly Color BlackColor = new(0f, 0f, 0f, 1f);

        public UnityEvent<bool> OnToggleValueChanged;

        private void Start()
        {
            _selectorMovementSpeed = MaxAbsSelectorPosX * 2f / SelectorMovementDuration;
            if (Toggled)
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(MaxAbsSelectorPosX, 0f);
                _frameImage.color = BlackColor;
                _selectorImage.color = WhiteColor;
            }
            else
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(-MaxAbsSelectorPosX, 0f);
                _frameImage.color = WhiteColor;
                _selectorImage.color = BlackColor;
            }
        }

        public void Toggle()
        {
            Toggled = !Toggled;
            // TODO: Add vibration
        }

        private void Update()
        {
            if (Toggled)
            {
                if (_selectorImage.rectTransform.anchoredPosition.x < MaxAbsSelectorPosX)
                {
                    _selectorImage.rectTransform.anchoredPosition += new Vector2(_selectorMovementSpeed * Time.deltaTime, 0f);
                    _frameImage.color -= new Color(SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, 0f);
                    _selectorImage.color += new Color(SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, 0f);
                }
                else if (_selectorImage.rectTransform.anchoredPosition.x > -MaxAbsSelectorPosX)
                {
                    _selectorImage.rectTransform.anchoredPosition = new Vector2(MaxAbsSelectorPosX, 0f);
                    _frameImage.color = BlackColor;
                    _selectorImage.color = WhiteColor;
                }
            }
            else
            {
                if (_selectorImage.rectTransform.anchoredPosition.x > -MaxAbsSelectorPosX)
                {
                    _selectorImage.rectTransform.anchoredPosition += new Vector2(-_selectorMovementSpeed * Time.deltaTime, 0f);
                    _frameImage.color += new Color(SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, 0f);
                    _selectorImage.color -= new Color(SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, SelectorMovementDuration * Time.deltaTime, 0f);
                }
                else if (_selectorImage.rectTransform.anchoredPosition.x < -MaxAbsSelectorPosX)
                {
                    _selectorImage.rectTransform.anchoredPosition = new Vector2(-MaxAbsSelectorPosX, 0f);
                    _frameImage.color = WhiteColor;
                    _selectorImage.color = BlackColor;
                }
            }
        }
    }
}
