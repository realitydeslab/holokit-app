using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_SignInPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "SignInPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _descriptionText;

        [SerializeField] private Button _signInButton;

        [SerializeField] private TMP_Text _notificationText;

        [SerializeField] private GameObject _userInfo;

        [SerializeField] private TMP_Text _emailText;

        private const string AppleUserIdKey = "AppleUserId";

        private const string AppleUserNameKey = "AppleUserName";

        private const string AppleUserEmailKey = "AppleUserEmail";

        private IAppleAuthManager _appleAuthManager;

        private void Awake()
        {
            _descriptionText.gameObject.SetActive(false);
            _signInButton.gameObject.SetActive(false);
            _signInButton.onClick.AddListener(OnSignInWithAppleButtonPressed);
            _notificationText.gameObject.SetActive(false);
            _userInfo.SetActive(false);
        }

        private void Start()
        {
            if (HoloKit.HoloKitUtils.IsEditor)
            {
                OnNotSignedIn();
                return;
            }

            // If the current platform is supported
            if (!AppleAuthManager.IsCurrentPlatformSupported) { return; }

            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            _appleAuthManager = new AppleAuthManager(deserializer);

            Initialize();
        }

        private void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (_appleAuthManager != null)
            {
                _appleAuthManager.Update();
            }
        }

        private void Initialize()
        {
            // Check if the current platform supports Sign In With Apple
            if (_appleAuthManager == null)
            {
                return;
            }

            // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
            _appleAuthManager.SetCredentialsRevokedCallback(result =>
            {
                Debug.Log("Received revoked callback " + result);
                OnNotSignedIn();
                PlayerPrefs.DeleteKey(AppleUserIdKey);
            });

            // If we have an Apple User Id available, get the credential status for it
            if (PlayerPrefs.HasKey(AppleUserIdKey))
            {
                var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
                OnCheckingCredentials();
                CheckCredentialStatusForUserId(storedAppleUserId);
            }
            // If we do not have an stored Apple User Id, attempt a quick login
            else
            {
                OnAttemptingQuickLogin();
                AttemptQuickLogin();
            }
        }

        private void CheckCredentialStatusForUserId(string appleUserId)
        {
            // If there is an apple ID available, we should check the credential state
            _appleAuthManager.GetCredentialState(
                appleUserId,
                state =>
                {
                    switch (state)
                    {
                        // If it's authorized, login with that user id
                        case CredentialState.Authorized:
                            OnSignedIn();
                            return;

                        // If it was revoked, or not found, we need a new sign in with apple attempt
                        // Discard previous apple user id
                        case CredentialState.Revoked:
                        case CredentialState.NotFound:
                            OnNotSignedIn();
                            PlayerPrefs.DeleteKey(AppleUserIdKey);
                            return;
                    }
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
                    OnNotSignedIn();
                });
        }

        private void AttemptQuickLogin()
        {
            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            _appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    // If it's an Apple credential, save the user ID, for later logins
                    if (credential is IAppleIDCredential appleIdCredential)
                    {
                        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                        PlayerPrefs.SetString(AppleUserNameKey, appleIdCredential.FullName.ToString());
                        PlayerPrefs.SetString(AppleUserEmailKey, appleIdCredential.Email.ToString());
                    }
                    Debug.Log($"[SignInWithApple] Quick logged in with user full name: {PlayerPrefs.GetString(AppleUserNameKey)} and email: {PlayerPrefs.GetString(AppleUserEmailKey)}");

                    OnSignedIn();
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    OnNotSignedIn();
                });
        }

        private void SignInWithApple()
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                    if (credential is IAppleIDCredential appleIdCredential)
                    {
                        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                        PlayerPrefs.SetString(AppleUserNameKey, appleIdCredential.FullName.ToString());
                        PlayerPrefs.SetString(AppleUserEmailKey, appleIdCredential.Email.ToString());
                    }
                    Debug.Log($"[SignInWithApple] Signed in with user full name: {PlayerPrefs.GetString(AppleUserNameKey)} and email: {PlayerPrefs.GetString(AppleUserEmailKey)}");

                    // TODO: Send user data to Unity backend

                    OnSignedInWithEmailDisplayed();
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    OnNotSignedIn();
                });
        }

        public void OnSignInWithAppleButtonPressed()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                OnSigningIn();
                SignInWithApple();
            }
            else
            {
                OnSignedIn();
            }
        }

        private void OnNotSignedIn()
        {
            _descriptionText.gameObject.SetActive(true);
            _signInButton.gameObject.SetActive(true);
            _signInButton.interactable = true;
            _notificationText.gameObject.SetActive(false);
        }

        private void OnCheckingCredentials()
        {
            _descriptionText.gameObject.SetActive(true);
            _signInButton.gameObject.SetActive(true);
            _signInButton.interactable = false;
            _notificationText.gameObject.SetActive(true);
            _notificationText.text = "Checking Apple Credentials...";
        }

        private void OnAttemptingQuickLogin()
        {
            _descriptionText.gameObject.SetActive(true);
            _signInButton.gameObject.SetActive(true);
            _signInButton.interactable = false;
            _notificationText.gameObject.SetActive(true);
            _notificationText.text = "Attempting Quick Login...";
        }

        private void OnSigningIn()
        {
            _descriptionText.gameObject.SetActive(true);
            _signInButton.gameObject.SetActive(true);
            _signInButton.interactable = false;
            _notificationText.gameObject.SetActive(true);
            _notificationText.text = "Signing In with Apple...";
        }

        private void OnSignedInWithEmailDisplayed()
        {
            _descriptionText.gameObject.SetActive(false);
            _signInButton.gameObject.SetActive(false);
            _notificationText.gameObject.SetActive(false);
            _userInfo.SetActive(true);
            _emailText.text = PlayerPrefs.GetString(AppleUserEmailKey);
            StartCoroutine(HoloKitAppUtils.WaitAndDo(2f, () =>
            {
                OnSignedIn();
            }));
        }

        private void OnSignedIn()
        {
            // Load next page
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityListPage");
        }
    }
}
