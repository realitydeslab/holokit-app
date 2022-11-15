using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private HoloKitAppUIPanel _dragonControllerUIPanel;

        [SerializeField] private GameObject _dragonControllerButton;

        private void Start()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        }

        public void OnDragonControllerButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel(_dragonControllerUIPanel);
        }
    }
}
