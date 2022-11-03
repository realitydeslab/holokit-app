using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityPreferencesPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityPreferencesPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _hostButton;

        [SerializeField] private GameObject _playerButton;

        [SerializeField] private GameObject _spectatorButton;

        [SerializeField] private GameObject _spacer;

        private void Start()
        {
            if (!HoloKitApp.Instance.CurrentReality.IsMultiplayerSupported())
            {
                _playerButton.SetActive(false);
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
            HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Host);
        }

        public void OnStartNonHostPlayerButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.NonHostPlayer);
        }

        public void OnStartSpectatorButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Spectator);
        }
    }
}
