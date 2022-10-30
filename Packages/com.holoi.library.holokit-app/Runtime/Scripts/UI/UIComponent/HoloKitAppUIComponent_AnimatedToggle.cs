using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_AnimatedToggle : MonoBehaviour
    {
        [SerializeField] private Image _frameDarkImage;

        [SerializeField] private Image _selectorImage;

        public bool Toggled;

        private const float MaxAbsSelectorPosX = 44f;

        private const float SelectorMovementDuration = 0.18f;

        public UnityEvent<bool> OnToggleValueChanged;

        private void Start()
        {
            if (Toggled)
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(MaxAbsSelectorPosX, 0f);
                _frameDarkImage.color = new Color(1f, 1f, 1f, 1f);
                _selectorImage.color = Color.white;
            }
            else
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(-MaxAbsSelectorPosX, 0f);
                _frameDarkImage.color = new Color(1f, 1f, 1f, 0f);
                _selectorImage.color = Color.black;
            }
        }

        public void Toggle()
        {
            LeanTween.cancel(_selectorImage.gameObject);
            LeanTween.cancel(_frameDarkImage.gameObject);
            if (Toggled)
            {
                LeanTween.moveX(_selectorImage.rectTransform, -MaxAbsSelectorPosX, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.color(_selectorImage.rectTransform, Color.black, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.alpha(_frameDarkImage.rectTransform, 0f, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                Toggled = false;
            }
            else
            {
                LeanTween.moveX(_selectorImage.rectTransform, MaxAbsSelectorPosX, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.color(_selectorImage.rectTransform, Color.white, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.alpha(_frameDarkImage.rectTransform, 1f, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                Toggled = true;
            }
        }
    }
}
