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

        public bool Authenticated => _authenticated;

        private bool _ugsInitialized = false;

        private bool _authenticated = false;

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

        public static event Action OnAuthenticatingAppleId;

        public static event Action OnAuthenticatingAppleIdSucceeded;

        public static event Action OnAuthenticatingAppleIdFailed;

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
                _authenticated = true;
                OnAuthenticatingAppleIdSucceeded?.Invoke();
            };

            AuthenticationService.Instance.SignInFailed += (error) =>
            {
                _authenticated = false;
                OnAuthenticatingAppleIdFailed?.Invoke();
                Debug.LogError(error);
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                _authenticated = false;
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                _authenticated = false;
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
                OnAuthenticatingAppleId?.Invoke();
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
