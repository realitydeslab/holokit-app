// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_SignInPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "SignInPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private Button _siwaButton;

        [SerializeField] private TMP_Text _userPromptText;

        private void Start()
        {
            if (!HoloKitApp.Instance.GlobalSettings.AppConfig.UserAccountSystemEnabled)
            {
                Authentication_OnSignedIn();
                return;
            }

            HoloKitAppUserAccountManager.SIWA_OnQuickLogin += SIWA_OnQuickLogin;
            HoloKitAppUserAccountManager.SIWA_OnSignInWithApple += SIWA_OnSignInWithApple;
            HoloKitAppUserAccountManager.SIWA_OnSignInFailed += SIWA_OnSignInFailed;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithCachedUser += Authentication_OnSignInWithCachedUser;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithApple += Authentication_OnSignInWithApple;
            HoloKitAppUserAccountManager.Authentication_OnSignedIn += Authentication_OnSignedIn;
            HoloKitAppUserAccountManager.Authentication_OnSignInFailed += Authentication_OnSignInFailed;

            if (HoloKit.HoloKitUtils.IsRuntime)
                HoloKitApp.Instance.UserAccountManager.Authentication_AttemptAutoLogin();
            else
                SIWA_OnSignInFailed();
        }

        private void OnDestroy()
        {
            HoloKitAppUserAccountManager.SIWA_OnQuickLogin -= SIWA_OnQuickLogin;
            HoloKitAppUserAccountManager.SIWA_OnSignInWithApple -= SIWA_OnSignInWithApple;
            HoloKitAppUserAccountManager.SIWA_OnSignInFailed -= SIWA_OnSignInFailed;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithCachedUser -= Authentication_OnSignInWithCachedUser;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithApple -= Authentication_OnSignInWithApple;
            HoloKitAppUserAccountManager.Authentication_OnSignedIn -= Authentication_OnSignedIn;
            HoloKitAppUserAccountManager.Authentication_OnSignInFailed -= Authentication_OnSignInFailed;
        }

        public void OnSIWAButtonPressed()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
                HoloKitApp.Instance.UserAccountManager.SIWA_SignInWithApple();
            else
                Authentication_OnSignedIn();
        }

        private void SIWA_OnQuickLogin()
        {
            _userPromptText.text = "Attempting Apple Quick Login...";
            _siwaButton.interactable = false;
        }

        private void SIWA_OnSignInWithApple()
        {
            _userPromptText.text = "Signing in with Apple...";
            _siwaButton.interactable = false;
        }

        private void SIWA_OnSignInFailed()
        {
            _userPromptText.text = "";
            _siwaButton.interactable = true;
        }

        private void Authentication_OnSignInWithCachedUser()
        {
            _userPromptText.text = "Authenticating cached user...";
            _siwaButton.interactable = false;
        }

        private void Authentication_OnSignInWithApple()
        {
            _userPromptText.text = "Authenticating Apple identity token...";
            _siwaButton.interactable = false;
        }

        private void Authentication_OnSignedIn()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            if (HoloKitApp.Instance.GlobalSettings.AppConfig.GalleryViewEnabled)
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityGalleryPage");
            else
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityPlainListPage");
        }

        private void Authentication_OnSignInFailed()
        {
            Authentication_OnSignedIn();
        }
    }
}
