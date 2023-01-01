using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase.UI;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingUIPanel : MofaUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private HoloKitAppUIPanel _dragonControllerUIPanel;

        protected override void Start()
        {
            HoloKitCamera.Instance.ForceScreenOrientation = !HoloKitApp.Instance.IsHost;

            if (HoloKitApp.Instance.IsHost)
                HoloKitApp.Instance.UIPanelManager.PushUIPanel(_dragonControllerUIPanel, HoloKitAppUICanvasType.Landscape);
            else if (HoloKitApp.Instance.IsSpectator)
                TriggerButton.gameObject.SetActive(false);
        }
    }
}
