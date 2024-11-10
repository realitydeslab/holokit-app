// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#if APPLE_SIGNIN_ENABLED

using System;
using System.Linq;
using System.Text;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public class HoloKitAppSIWAManager : MonoBehaviour
    {
        private IAppleAuthManager _appleAuthManager;

        public const string AppleUserIdKey = "AppleUserId";

        public const string AppleUserFullNameKey = "AppleUserName";

        public const string AppleUserEmailKey = "AppleUserEmail";

        public static event Action OnAttemptingQuickLogin;

        public static event Action OnAttemptingQuickLoginFailed;

        public static event Action OnSigningInWithApple;

        public static event Action<string> OnSignedInWithApple;

        public static event Action OnSigningInWithAppleFailed;

        private void Update()
        {
            // Updates the AppleAuthManager instance to execute pending callbacks inside Unity's execution loop
            if (_appleAuthManager != null)
            {
                _appleAuthManager.Update();
            }
        }

        public void SignIn()
        {
            // If the current platform is supported
            if (!AppleAuthManager.IsCurrentPlatformSupported)
            {
                Debug.Log("Current platform does not support SIWA");
                return;
            }

            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            _appleAuthManager = new AppleAuthManager(deserializer);

            // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
            _appleAuthManager.SetCredentialsRevokedCallback(result =>
            {
                // TODO: Handle credential revoke event
            });

            AttemptQuickLogin();

            // If we have an Apple User Id available, get the credential status for it
            //if (PlayerPrefs.HasKey(AppleUserIdKey))
            //{
            //    var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
            //    CheckCredentialStatusForUserId(storedAppleUserId);
            //}
            //// If we do not have an stored Apple User Id, attempt a quick login
            //else
            //{
            //    AttemptQuickLogin();
            //}
        }

        //private void CheckCredentialStatusForUserId(string appleUserId)
        //{
        //    //OnCheckCredentialStatus?.Invoke();
        //    // If there is an apple ID available, we should check the credential state
        //    _appleAuthManager.GetCredentialState(
        //        appleUserId,
        //        state =>
        //        {
        //            switch (state)
        //            {
        //                // If it's authorized, login with that user id
        //                case CredentialState.Authorized:
                            
        //                    return;
        //                // If it was revoked, or not found, we need a new sign in with apple attempt
        //                // Discard previous apple user id
        //                case CredentialState.Revoked:
        //                case CredentialState.NotFound:
        //                    PlayerPrefs.DeleteKey(AppleUserIdKey);
        //                    return;
        //            }
        //        },
        //        error =>
        //        {
        //            var authorizationErrorCode = error.GetAuthorizationErrorCode();
        //            Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
        //        });
        //}

        private void AttemptQuickLogin()
        {
            OnAttemptingQuickLogin?.Invoke();
            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            _appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    OnReceivedCredential(credential);
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    OnAttemptingQuickLoginFailed?.Invoke();
                });
        }

        public void SignInWithApple()
        {
            OnSigningInWithApple?.Invoke();
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    OnReceivedCredential(credential);
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    // TODO: This might be because there is no network connection, handle this
                    OnSigningInWithAppleFailed?.Invoke();
                });
        }

        private void OnReceivedCredential(ICredential credential)
        {
            if (credential is IAppleIDCredential appleIdCredential)
            {
                // Apple User ID
                // You should save the user ID somewhere in the device
                // This Apple User ID is constant and not changed for a user
                var userId = appleIdCredential.User;
                PlayerPrefs.SetString(AppleUserIdKey, userId);
                Debug.Log($"[SIWA] Apple User ID: {userId}");

                // Email (Received ONLY in the first login)
                var email = appleIdCredential.Email;
                if (email != null)
                {
                    PlayerPrefs.SetString(AppleUserEmailKey, email);
                    Debug.Log($"[SIWA] Email: {email}");
                }
                else
                {
                    Debug.Log("[SIWA] Cannot get user's email");
                }

                // Full name (Received ONLY in the first login)
                var fullName = appleIdCredential.FullName;
                if (fullName != null)
                {
                    string displayName = fullName.ToLocalizedString();
                    PlayerPrefs.SetString(AppleUserFullNameKey, displayName);
                    Debug.Log($"[SIWA] DisplayName: {displayName}");
                }
                else
                {
                    Debug.Log("[SIWA] Cannot get user's fullname");
                }

                // Identity token, this is different for each login
                var identityToken = Encoding.UTF8.GetString(
                    appleIdCredential.IdentityToken,
                    0,
                    appleIdCredential.IdentityToken.Length);
                Debug.Log($"[SIWA] IdentityToken: {identityToken}");

                // Authorization code, this is different for each login
                var authorizationCode = Encoding.UTF8.GetString(
                    appleIdCredential.AuthorizationCode,
                    0,
                    appleIdCredential.AuthorizationCode.Length);
                Debug.Log($"[SIWA] AuthorizationCode: {authorizationCode}");

                OnSignedInWithApple?.Invoke(identityToken);
            }
        }
    }
}
#endif