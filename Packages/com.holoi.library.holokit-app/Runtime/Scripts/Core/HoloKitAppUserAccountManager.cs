using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUserAccountManager : MonoBehaviour
    {
        public HoloKitAppSIWAManager SIWAManager;

        [SerializeField] private bool _clearSessionToken = false;

        private bool _ugsInitialized;

        /// <summary>
        /// This event is called when we start to initialize UGS.
        /// </summary>
        public static event Action OnInitializingUGS;

        /// <summary>
        /// This event is called when UGS initialization fails.
        /// </summary>
        public static event Action OnInitializingUGSFailed;

        /// <summary>
        /// This event is called when we attempt to sign in with a cached user.
        /// </summary>
        public static event Action OnSigningInWithCachedUser;

        public static event Action OnAuthenticatingAppleUserId;

        public static event Action OnAuthenticatingAppleUserIdSucceeded;

        public static event Action OnAuthenticatingAppleUserIdFailed;

        private void Start()
        {
            HoloKitAppSIWAManager.OnSignedInWithApple += OnSignedInWithApple;
        }

        private void OnDestroy()
        {
            HoloKitAppSIWAManager.OnSignedInWithApple -= OnSignedInWithApple;
        }

        private void SetupAuthenticationCallbacks()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("[Authentication] Signed in");
                OnAuthenticatingAppleUserIdSucceeded?.Invoke();
            };

            AuthenticationService.Instance.SignInFailed += (error) =>
            {
                Debug.LogError(error);
                OnAuthenticatingAppleUserIdFailed?.Invoke();
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        public async void StartProcess()
        {
            _ugsInitialized = await InitializeUGS();
            if (_ugsInitialized)
            {
                if (_clearSessionToken)
                    AuthenticationService.Instance.ClearSessionToken();
                SetupAuthenticationCallbacks();
                StartAuthentication();
            }
        }

        private async Task<bool> InitializeUGS()
        {
            try
            {
                OnInitializingUGS?.Invoke();
                await UnityServices.InitializeAsync();
                Debug.Log("UGS initialized");
                return true;
            }
            catch (Exception e)
            {
                OnInitializingUGSFailed?.Invoke();
                Debug.Log("Failed to initialize UGS");
                Debug.LogException(e);
                return false;
            }
        }

        private async void StartAuthentication()
        {
            // Check if a cached user already exists by checking if the session token exists
            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                // Sign in with Apple
                SIWAManager.SignIn();
                return;
            }

            // Sign in Anonymously
            // This call will sign in the cached user.
            try
            {
                OnSigningInWithCachedUser?.Invoke();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException e)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
            catch (RequestFailedException e)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
        }

        private async void OnSignedInWithApple(string idToken)
        {
            try
            {
                OnAuthenticatingAppleUserId?.Invoke();
                await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
            }
            catch (AuthenticationException e)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
            catch (RequestFailedException e)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
        }
    }
}
