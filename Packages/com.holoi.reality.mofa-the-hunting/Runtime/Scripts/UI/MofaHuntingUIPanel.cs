using System;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private HoloKitAppUIPanel _dragonControllerUIPanel;

        [SerializeField] private GameObject _spawnDragonButton;

        [SerializeField] private GameObject _dragonControllerButton;

        public static event Action OnSpawnDragonButtonPressed;

        private void Start()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            _dragonControllerButton.SetActive(false);
            if (!HoloKitApp.Instance.IsHost && !HoloKitApp.Instance.IsPuppeteer)
            {
                _spawnDragonButton.SetActive(false);
            }

            DragonController.OnDragonSpawned += OnDragonSpawned;
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;
        }

        private void OnDestroy()
        {
            DragonController.OnDragonSpawned -= OnDragonSpawned;
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
        }

        public void OnSpawnDragonButtonPressedFunc()
        {
            OnSpawnDragonButtonPressed?.Invoke();
        }

        private void OnDragonSpawned()
        {
            _spawnDragonButton.SetActive(false);
            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                _dragonControllerButton.SetActive(true);
            }
        }

        public void OnDragonControllerButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel(_dragonControllerUIPanel, HoloKitAppUICanvasType.Landscape);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        private void OnMofaPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Waiting)
            {
                _dragonControllerButton.SetActive(false);
                _spawnDragonButton.SetActive(true);
            }
        }
    }
}
