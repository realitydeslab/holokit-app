// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Services.Authentication;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// This part of the partial class is responsible for Authentication.
    /// </summary>
    public partial class HoloKitAppUserAccountManager
    {
        // https://docs.unity.com/authentication/AuthenticationSession.html
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;

        public bool IsAuthorized => AuthenticationService.Instance.IsAuthorized;

        public bool IsExpired => AuthenticationService.Instance.IsExpired;

        /// <summary>
        /// This event is called when we attempt to sign in with the cached user.
        /// </summary>
        public static event Action Authentication_OnSignInWithCachedUser;

        /// <summary>
        /// This event is called when we attempt to authenticate Apple identity token.
        /// </summary>
        public static event Action Authentication_OnSignInWithApple;

        /// <summary>
        /// This event is called when the user successfully logged in with Unity Authentication.
        /// </summary>
        public static event Action Authentication_OnSignedIn;

        /// <summary>
        /// This event is called when the user failed to login with Unity Authentication.
        /// The only known reason to fail now is lack of network connection.
        /// </summary>
        public static event Action Authentication_OnSignInFailed;

        private void Authentication_Init()
        {
            if (_clearSessionToken)
                AuthenticationService.Instance.ClearSessionToken();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Unity Player ID: {AuthenticationService.Instance.PlayerInfo.Id}");
                Authentication_OnSignedIn?.Invoke();
                CloudSave_UploadAppleUserData();
            };

            AuthenticationService.Instance.SignInFailed += (error) =>
            {
                // This will fire when there is no network connection
                Authentication_OnSignInFailed?.Invoke();
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("[Authentication] Signed out");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("[Authentication] Expired");
            };
        }

        public async void Authentication_AttemptAutoLogin()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                try
                {
                    Authentication_OnSignInWithCachedUser?.Invoke();
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                catch (Exception)
                {
                    // This will fire when there is no network connection
                    Debug.Log("[Authentication] Exception: Sign in with cached user failed");
                }
            }
            else
            {
                SIWA_AttemptQuickLogin();
                return;
            }
        }

        private async void Authentication_SignInWithApple(string identityToken)
        {
            try
            {
                Authentication_OnSignInWithApple?.Invoke();
                await AuthenticationService.Instance.SignInWithAppleAsync(identityToken);
            }
            catch (Exception)
            {
                // This will fire when there is no network connection
                Debug.Log("[Authentication] Exception: Sign in with Apple failed");
            }
        }
    }
}
