using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingDragonControllerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MofaHuntingDragonController";

        public override bool OverlayPreviousPanel => true;

        private void Start()
        {
            HoloKitCamera.Instance.ForceScreenOrientation = false;
        }

        private void Update()
        {
            if (Screen.orientation != ScreenOrientation.LandscapeLeft)
                Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        private void OnDestroy()
        {
            HoloKitCamera.Instance.ForceScreenOrientation = true;
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
