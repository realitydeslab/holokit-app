using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_LandingPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "LandingPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private Image _logoImage;

        [SerializeField] private Image _sloganImage;

        [SerializeField] private Image _headsetImage;

        private const float PageDuration = 2f;

        private const float LogoAppearTime = 0.6f;

        private const float SloganAppearDelay = 0.6f;

        private const float SloganAppearTime = 0.6f;

        private const float HeadsetEndPosY = -960f;

        private const float HeadsetMovementDuration = 1.2f;

        private void Start()
        {
            // Logo
            _logoImage.color = new Color(1f, 1f, 1f, 0f);
            LeanTween.alpha(_logoImage.rectTransform, 1f, LogoAppearTime)
                .setEase(LeanTweenType.easeInOutSine);

            // Slogan
            _logoImage.color = new Color(1f, 1f, 1f, 0f);
            LeanTween.alpha(_sloganImage.rectTransform, 1f, SloganAppearTime)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(SloganAppearDelay);

            // Headset
            LeanTween.moveY(_headsetImage.rectTransform, HeadsetEndPosY, HeadsetMovementDuration)
                .setEase(LeanTweenType.easeSpring);

            StartCoroutine(HoloKitAppUtils.WaitAndDo(PageDuration, () =>
            {
                LoadPermissionPage();
            }));
        }

        private void LoadPermissionPage()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("PermissionsPage");
        }
    }
}
