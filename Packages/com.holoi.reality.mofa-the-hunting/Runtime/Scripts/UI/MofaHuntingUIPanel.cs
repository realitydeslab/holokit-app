using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase.UI;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingUIPanel : MofaUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private HoloKitAppUIPanel _dragonControllerUIPanel;

        protected override void Start()
        {
            if (HoloKitApp.Instance.IsHost)
                HoloKitApp.Instance.UIPanelManager.PushUIPanel(_dragonControllerUIPanel, HoloKitAppUICanvasType.Landscape);
            else if (HoloKitApp.Instance.IsSpectator)
                TriggerButton.gameObject.SetActive(false);
        }

        //private void OnDragonSpawned()
        //{
        //    _spawnDragonButton.SetActive(false);
        //    if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
        //    {
        //        _dragonControllerButton.SetActive(true);
        //    }
        //}

        //private void OnMofaPhaseChanged(MofaPhase mofaPhase)
        //{
        //    if (mofaPhase == MofaPhase.Waiting)
        //    {
        //        _dragonControllerButton.SetActive(false);
        //        _spawnDragonButton.SetActive(true);
        //    }
        //}
    }
}
