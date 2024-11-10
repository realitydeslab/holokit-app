// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#if APPLE_SIGNIN_ENABLED

using System;
using System.Text;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// This part of the partial class is responsible for SignInWithApple plugin.
    /// </summary>
    public partial class HoloKitAppUserAccountManager
    {
        private IAppleAuthManager _appleAuthManager;

        private const string AppleUserIdKey = "AppleUserId";
        private const string AppleUserEmailKey = "AppleUserEmail";
        private const string AppleUserNameKey = "AppleUserName";

        /// <summary>
        /// This event is called when we attempt quick login.
        /// </summary>
        public static event Action SIWA_OnQuickLogin;

        /// <summary>
        /// This event is called when we attempt sign in with Apple.
        /// </summary>
        public static event Action SIWA_OnSignInWithApple;

        /// <summary>
        /// This event is called either quick login or sign in with Apple fails.
        /// </summary>
        public static event Action SIWA_OnSignInFailed;

        private void SIWA_Init()
        {
            if (HoloKit.HoloKitUtils.IsEditor)
                return;

            if (!AppleAuthManager.IsCurrentPlatformSupported)
            {
                Debug.Log("[SIWA] Current platform does not support SIWA");
                return;
            }

            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            _appleAuthManager = new AppleAuthManager(deserializer);

            // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
            _appleAuthManager.SetCredentialsRevokedCallback(result =>
            {
                Debug.Log("[SIWA] Received revoked callback " + result);
            });
        }

        private void SIWA_Deinit()
        {
            if (_appleAuthManager != null)
                _appleAuthManager = null;
        }

        private void SIWA_Update()
        {
            // Update the AppleAuthManager instance to execute pending callbacks inside Unity's execution loop
            if (_appleAuthManager != null)
            {
                _appleAuthManager.Update();
            }
        }

        private void SIWA_AttemptQuickLogin()
        {
            SIWA_OnQuickLogin?.Invoke();

            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            _appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    SIWA_OnReceivedCredential(credential);
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("[SIWA] Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    SIWA_OnSignInFailed?.Invoke();
                });
        }

        public void SIWA_SignInWithApple()
        {
            SIWA_OnSignInWithApple?.Invoke();

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    SIWA_OnReceivedCredential(credential);
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("[SIWA] Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    SIWA_OnSignInFailed?.Invoke();
                });
        }

        private void SIWA_OnReceivedCredential(ICredential credential)
        {
            if (credential is IAppleIDCredential appleIdCredential)
            {
                // AppleUserId is the same for each login
                string appleUserId = appleIdCredential.User;
                string appleUserEmail = appleIdCredential.Email;
                string appleUserName = appleIdCredential.FullName?.ToLocalizedString();
                // IdentityToken and AuthorizationCode are different for each login
                var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);

                // Save Apple user data to local storage
                PlayerPrefs.SetString(AppleUserIdKey, appleUserId);
                if (appleUserEmail != null)
                    PlayerPrefs.SetString(AppleUserEmailKey, appleUserEmail);
                if (appleUserName != null)
                    PlayerPrefs.SetString(AppleUserNameKey, appleUserName);

                // Send OnPlayerRegistered event to Unity Analytics
                if (appleUserEmail != null)
                    HoloKitAppAnalyticsEventManager.FireEvent_OnPlayerRegistered(appleUserEmail, appleUserName);

                // Sign in with Unity Authentication using Apple identity token
                #if UNITY_SERVICES_CORE_ENABLED
                Authentication_SignInWithApple(identityToken);
                #endif

                // Deinitialize SIWA since we no longer need it
                SIWA_Deinit();
            }
        }
    }
}
#endif