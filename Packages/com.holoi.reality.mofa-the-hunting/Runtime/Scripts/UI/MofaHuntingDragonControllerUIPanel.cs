using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingDragonControllerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MofaHuntingDragonController";

        public override bool OverlayPreviousPanel => true;

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
