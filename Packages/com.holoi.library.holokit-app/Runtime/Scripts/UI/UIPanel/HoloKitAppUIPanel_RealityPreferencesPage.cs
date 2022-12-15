using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityPreferencesPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityPreferencesPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _hostButton;

        [SerializeField] private GameObject _hostPuppeteerButton;

        [SerializeField] private GameObject _hostPlayerButton;

        [SerializeField] private GameObject _playerButton;

        [SerializeField] private GameObject _puppeteerButton;

        [SerializeField] private GameObject _spectatorButton;

        [SerializeField] private GameObject _spacer;

        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (currentReality.IsHostPuppeteerSupported())
            {
                _hostButton.SetActive(false);
                _hostPuppeteerButton.SetActive(true);
            }
            else
            {
                _hostPuppeteerButton.SetActive(false);
            }

            if (currentReality.IsHostMultiplayerSupported())
            {
                _hostButton.SetActive(false);
                _hostPlayerButton.SetActive(true);
            }
            else
            {
                _hostPlayerButton.SetActive(false);
            }

            if (!HoloKitApp.Instance.CurrentReality.IsMultiplayerSupported())
            {
                _playerButton.SetActive(false);
            }
            if (!HoloKitApp.Instance.CurrentReality.IsPuppeteerSupported())
            {
                _puppeteerButton.SetActive(false);
            }
            if (!HoloKitApp.Instance.CurrentReality.IsSpectatorViewSupported())
            {
                _spectatorButton.SetActive(false);
            }

            if (HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList().Count == 0
                && HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaObjectCollectionList().Count == 0)
            {
                _spacer.SetActive(true);
            }
            else
            {
                _spacer.SetActive(false);
            }
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnStartHostButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(true, HoloKitAppPlayerType.Player);
        }

        public void OnStartNonHostPlayerButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(false, HoloKitAppPlayerType.Player);
        }

        public void OnStartPuppeteerButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(false, HoloKitAppPlayerType.Puppeteer);
        }

        public void OnStartSpectatorButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(false, HoloKitAppPlayerType.Spectator);
        }
    }
}
