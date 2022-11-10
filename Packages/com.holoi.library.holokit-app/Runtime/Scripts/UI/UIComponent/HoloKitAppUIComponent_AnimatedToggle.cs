using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Holoi.Library.HoloKitApp.UI
{
    public enum AnimatedToggleColorTheme
    {
        Black = 0,
        White = 1
    }

    public class HoloKitAppUIComponent_AnimatedToggle : MonoBehaviour
    {
        [SerializeField] private Image _frameUnselectedImage;

        [SerializeField] private Image _frameSelectedImage;

        [SerializeField] private Image _selectorImage;

        [SerializeField] private AnimatedToggleColorTheme _colorTheme;

        public bool Toggled;

        private const float MaxAbsSelectorPosX = 44f;

        private const float SelectorMovementDuration = 0.18f;

        public UnityEvent<bool> OnToggleValueChanged;

        private void Start()
        {
            if (_colorTheme == AnimatedToggleColorTheme.Black)
            {
                _frameUnselectedImage.color = Color.black;
                _frameSelectedImage.color = Color.black;
                _selectorImage.color = Color.black;
            }
            else
            {
                _frameUnselectedImage.color = Color.white;
                _frameSelectedImage.color = Color.white;
                _selectorImage.color = Color.white;
            }

            if (Toggled)
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(MaxAbsSelectorPosX, 0f);
                _frameSelectedImage.color = new Color(_frameSelectedImage.color.r, _frameSelectedImage.color.g, _frameSelectedImage.color.b, 1f);
                _selectorImage.color = _colorTheme == AnimatedToggleColorTheme.Black ?
                    Color.white : Color.black;
            }
            else
            {
                _selectorImage.rectTransform.anchoredPosition = new Vector2(-MaxAbsSelectorPosX, 0f);
                _frameSelectedImage.color = new Color(_frameSelectedImage.color.r, _frameSelectedImage.color.g, _frameSelectedImage.color.b, 0f);
                _selectorImage.color = new(_selectorImage.color.r, _selectorImage.color.g, _selectorImage.color.b, 1f);
            }
        }

        public void Toggle()
        {
            LeanTween.cancel(_selectorImage.gameObject);
            LeanTween.cancel(_frameSelectedImage.gameObject);
            if (Toggled)
            {
                LeanTween.moveX(_selectorImage.rectTransform, -MaxAbsSelectorPosX, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.alpha(_frameSelectedImage.rectTransform, 0f, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.color(_selectorImage.rectTransform, _colorTheme == AnimatedToggleColorTheme.Black ? Color.black : Color.white, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                Toggled = false;
                OnToggleValueChanged?.Invoke(Toggled);
            }
            else
            {
                LeanTween.moveX(_selectorImage.rectTransform, MaxAbsSelectorPosX, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.alpha(_frameSelectedImage.rectTransform, 1f, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                LeanTween.color(_selectorImage.rectTransform, _colorTheme == AnimatedToggleColorTheme.Black ? Color.white : Color.black, SelectorMovementDuration)
                    .setEase(LeanTweenType.easeInOutSine);
                Toggled = true;
                OnToggleValueChanged?.Invoke(Toggled);
            }
        }
    }
}
