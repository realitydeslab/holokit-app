using System;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingDragonControllerUIPanel : HoloKitAppUIPanel_MonoAR
    {
        public override string UIPanelName => "MofaHuntingDragonController";

        public override bool OverlayPreviousPanel => true;

        [Header("MOFA: The Hunting")]
        [SerializeField] private RectTransform _spawnDragonButton;

        [SerializeField] private RectTransform _monoPanel;

        [SerializeField] private RectTransform _dragonControlPanel;

        private const float SpawnDragonButtonRotationSpeed = 20f;

        public static event Action OnSpawnDragonButtonPressed;

        public static event Action OnDragonPrimaryAttackButtonPressed;

        public static event Action OnDragonSecondaryAttackButtonPressed;

        public static event Action OnDragonLockButtonPressed;

        private void Start()
        {
            MofaHuntingRealityManager.OnDragonSpawned += OnDragonSpawned;
            MofaHuntingRealityManager.OnDragonDied += OnDragonDied;

            _spawnDragonButton.gameObject.SetActive(true);
            _monoPanel.gameObject.SetActive(true);
            _dragonControlPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            MofaHuntingRealityManager.OnDragonSpawned -= OnDragonSpawned;
            MofaHuntingRealityManager.OnDragonDied -= OnDragonDied;
        }

        private void Update()
        {
            if (Screen.orientation != ScreenOrientation.LandscapeLeft)
                Screen.orientation = ScreenOrientation.LandscapeLeft;

            // Rotate the spawn dragon button if it is active
            if (_spawnDragonButton.gameObject.activeSelf)
            {
                _spawnDragonButton.Rotate(new Vector3(0f, 0f, 1f), -SpawnDragonButtonRotationSpeed * Time.deltaTime);
            }
        }

        public void OnSpawnDragonButtonPressedFunc()
        {
            OnSpawnDragonButtonPressed?.Invoke();
        }

        private void OnDragonSpawned()
        {
            _spawnDragonButton.gameObject.SetActive(false);
            _dragonControlPanel.gameObject.SetActive(true);
        }

        private void OnDragonDied()
        {
            _spawnDragonButton.gameObject.SetActive(true);
            _dragonControlPanel.gameObject.SetActive(false);
        }

        public void OnDragonPrimaryAttackButtonPressedFunc()
        {
            OnDragonPrimaryAttackButtonPressed?.Invoke();
        }

        public void OnDragonSecondaryAttackButtonPressedFunc()
        {
            OnDragonSecondaryAttackButtonPressed?.Invoke();
        }

        public void OnDragonLockButtonPressedFunc()
        {
            OnDragonLockButtonPressed?.Invoke();
        }
    }
}
