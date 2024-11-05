// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_LandingPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "LandingPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private Image _logoImage;

        [SerializeField] private Image _sloganImage;

        [SerializeField] private Image _headsetImage;

        private const float PageDuration = 2f;

        private const float LogoAppearTime = 1.2f;

        private const float SloganAppearTime = 1.2f;

        private const float HeadsetEndPosY = 1550f;

        private const float HeadsetMovementDuration = 1.2f;

        private void Start()
        {
            if (!HoloKitApp.Instance.GlobalSettings.AppConfig.LandingPageEnabled) {
                LoadPermissionPage();
            } else {
                // Logo
                _logoImage.color = new Color(1f, 1f, 1f, 0f);
                LeanTween.alpha(_logoImage.rectTransform, 1f, LogoAppearTime)
                    .setEase(LeanTweenType.easeInOutSine);

                // Slogan
                _sloganImage.color = new Color(1f, 1f, 1f, 0f);
                LeanTween.alpha(_sloganImage.rectTransform, 1f, SloganAppearTime)
                    .setEase(LeanTweenType.easeInOutSine);

                // Headset
                LeanTween.moveY(_headsetImage.rectTransform, HeadsetEndPosY, HeadsetMovementDuration)
                    .setEase(LeanTweenType.easeSpring);

                StartCoroutine(HoloKitAppUtils.WaitAndDo(PageDuration, () =>
                {
                    LoadPermissionPage();
                }));
            }
        }

        private void LoadPermissionPage()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("PermissionsPage");
        }
    }
}
