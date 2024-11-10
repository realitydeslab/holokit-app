// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
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
        #if !APPLE_SIGNIN_ENABLED 
            Authentication_OnSignedIn();
            return;
        #endif
            if (!HoloKitApp.Instance.GlobalSettings.AppConfig.UserAccountSystemEnabled)
            {
                Authentication_OnSignedIn();
                return;
            }

        #if APPLE_SIGNIN_ENABLED
            HoloKitAppUserAccountManager.SIWA_OnQuickLogin += SIWA_OnQuickLogin;
            HoloKitAppUserAccountManager.SIWA_OnSignInWithApple += SIWA_OnSignInWithApple;
            HoloKitAppUserAccountManager.SIWA_OnSignInFailed += SIWA_OnSignInFailed;
        #endif
        #if UNITY_SERVICES_CORE_ENABLED
            HoloKitAppUserAccountManager.Authentication_OnSignInWithCachedUser += Authentication_OnSignInWithCachedUser;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithApple += Authentication_OnSignInWithApple;
            HoloKitAppUserAccountManager.Authentication_OnSignedIn += Authentication_OnSignedIn;
            HoloKitAppUserAccountManager.Authentication_OnSignInFailed += Authentication_OnSignInFailed;
        #endif 

        #if APPLE_SIGNIN_ENABLED && UNITY_SERVICES_CORE_ENABLED
            if (HoloKit.HoloKitUtils.IsRuntime)
                HoloKitApp.Instance.UserAccountManager.Authentication_AttemptAutoLogin();
            else
                SIWA_OnSignInFailed();
        #endif
        }

        private void OnDestroy()
        {
        #if APPLE_SIGNIN_ENABLED
            HoloKitAppUserAccountManager.SIWA_OnQuickLogin -= SIWA_OnQuickLogin;
            HoloKitAppUserAccountManager.SIWA_OnSignInWithApple -= SIWA_OnSignInWithApple;
            HoloKitAppUserAccountManager.SIWA_OnSignInFailed -= SIWA_OnSignInFailed;
        #endif 
        #if UNITY_SERVICES_CORE_ENABLED
            HoloKitAppUserAccountManager.Authentication_OnSignInWithCachedUser -= Authentication_OnSignInWithCachedUser;
            HoloKitAppUserAccountManager.Authentication_OnSignInWithApple -= Authentication_OnSignInWithApple;
            HoloKitAppUserAccountManager.Authentication_OnSignedIn -= Authentication_OnSignedIn;
            HoloKitAppUserAccountManager.Authentication_OnSignInFailed -= Authentication_OnSignInFailed;
        #endif
        }

        public void OnSIWAButtonPressed()
        {
        #if APPLE_SIGNIN_ENABLED
            if (HoloKit.HoloKitUtils.IsRuntime)
                HoloKitApp.Instance.UserAccountManager.SIWA_SignInWithApple();
            else
        #endif
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
