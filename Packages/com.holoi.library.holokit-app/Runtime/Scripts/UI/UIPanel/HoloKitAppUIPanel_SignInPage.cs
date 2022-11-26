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

        [SerializeField] private TMP_Text _processText;

        private void Start()
        {
            HoloKitAppUserAccountManager.OnInitializingUGS += OnInitializingUGS;
            HoloKitAppUserAccountManager.OnInitializingUGSFailed += OnInitializingUGSFailed;
            HoloKitAppUserAccountManager.OnSigningInWithCachedUser += OnSigningInWithCachedUser;
            HoloKitAppSIWAManager.OnAttemptingQuickLogin += OnAttemptingQuickLogin;
            HoloKitAppSIWAManager.OnAttemptingQuickLoginFailed += OnAttemptingQuickLoginFailed;
            HoloKitAppSIWAManager.OnSigningInWithApple += OnSigningInWithApple;
            HoloKitAppSIWAManager.OnSigningInWithAppleFailed += OnSigningInWithAppleFailed;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleId += OnAuthenticatingAppleId;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleIdSucceeded += OnAuthenticatingAppleIdSucceeded;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleIdFailed += OnAuthenticatingAppleIdFailed;

            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                HoloKitApp.Instance.UserAccountManager.StartProcess();
            }
            else
            {
                OnAttemptingQuickLoginFailed();
            }  
        }

        private void OnDestroy()
        {
            HoloKitAppUserAccountManager.OnInitializingUGS += OnInitializingUGS;
            HoloKitAppUserAccountManager.OnInitializingUGSFailed += OnInitializingUGSFailed;
            HoloKitAppUserAccountManager.OnSigningInWithCachedUser += OnSigningInWithCachedUser;
            HoloKitAppSIWAManager.OnAttemptingQuickLogin += OnAttemptingQuickLogin;
            HoloKitAppSIWAManager.OnAttemptingQuickLoginFailed += OnAttemptingQuickLoginFailed;
            HoloKitAppSIWAManager.OnSigningInWithApple += OnSigningInWithApple;
            HoloKitAppSIWAManager.OnSigningInWithAppleFailed += OnSigningInWithAppleFailed;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleId += OnAuthenticatingAppleId;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleIdSucceeded += OnAuthenticatingAppleIdSucceeded;
            HoloKitAppUserAccountManager.OnAuthenticatingAppleIdFailed += OnAuthenticatingAppleIdFailed;
        }

        public void OnSIWAButtonPressed()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                HoloKitApp.Instance.UserAccountManager.SIWAManager.SignInWithApple();
            }
            else
            {
                OnAuthenticatingAppleIdSucceeded();
            }
        }

        private void OnInitializingUGS()
        {
            _siwaButton.interactable = false;
            _processText.text = "Initializing Unity Game Services...";
        }

        private void OnInitializingUGSFailed()
        {
            _siwaButton.interactable = false;
            _processText.text = "Failed to initialize Unity Gaming Services";
        }

        private void OnSigningInWithCachedUser()
        {
            _siwaButton.interactable = false;
            _processText.text = "Signing in with cached user...";
        }

        private void OnAttemptingQuickLogin()
        {
            _siwaButton.interactable = false;
            _processText.text = "Attempting Apple Quick Login...";
        }

        private void OnAttemptingQuickLoginFailed()
        {
            _siwaButton.interactable = true;
            _processText.text = "";
        }

        private void OnSigningInWithApple()
        {
            _siwaButton.interactable = false;
            _processText.text = "Signing in with Apple...";
        }

        private void OnSigningInWithAppleFailed()
        {
            _siwaButton.interactable = true;
            _processText.text = "";
        }

        private void OnAuthenticatingAppleId()
        {
            _siwaButton.interactable = false;
            _processText.text = "Authenticating Apple ID...";
        }

        private void OnAuthenticatingAppleIdSucceeded()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityListPage");
        }

        private void OnAuthenticatingAppleIdFailed()
        {
            //_siwaButton.interactable = false;
            //_processText.text = "Authentication failed";

            // Enter as offline mode
            OnAuthenticatingAppleIdSucceeded();
        }
    }
}
