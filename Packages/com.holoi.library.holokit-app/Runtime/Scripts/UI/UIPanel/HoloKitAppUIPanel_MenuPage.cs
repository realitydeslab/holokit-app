using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MenuPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MenuPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnRealitiesButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnIntroButtonPressed()
        {

        }

        public void OnBuyHoloKitButtonPressed()
        {

        }

        public void OnGettingStartedButtonPressed()
        {

        }

        public void OnMyAccountButtonPressed()
        {

        }

        public void OnSettingsButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("SettingsPage");
        }

        public void OnTwitterButtonPressed()
        {

        }

        public void OnDiscordButtonPressed()
        {

        }

        public void OnInstagramButtonPressed()
        {

        }

        public void OnTiktokButtonPressed()
        {

        }

        public void OnYoutubeButtonPressed()
        {

        }

        public void OnMirrorButtonPressed()
        {

        }
    }
}
