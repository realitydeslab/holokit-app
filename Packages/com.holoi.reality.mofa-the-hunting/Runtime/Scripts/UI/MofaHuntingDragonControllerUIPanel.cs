using System;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using MalbersAnimations.Events;

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

        [SerializeField] private RectTransform _cancelButton;

        [SerializeField] private Slider _flyingSlider;

        [Header("MEvents")]
        [SerializeField] private MEvent _setFly;

        [SerializeField] private MEvent _setMovementMobile;

        private const float SpawnDragonButtonRotationSpeed = 20f;

        public static event Action OnSpawnDragonButtonPressed;

        public static event Action OnDragonPrimaryAttackButtonPressed;

        public static event Action OnDragonSecondaryAttackButtonPressed;

        public static event Action OnDragonLockButtonPressed;

        public static event Action OnDragonCancelLockButtonPressed;

        private void Start()
        {
            MofaHuntingRealityManager.OnDragonSpawned += OnDragonSpawned;
            MofaHuntingRealityManager.OnDragonDied += OnDragonDied;
            MofaHuntingRealityManager.OnTargetLocked += OnLockTargetSessionEnded;

            _spawnDragonButton.gameObject.SetActive(true);
            _monoPanel.gameObject.SetActive(true);
            _dragonControlPanel.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            MofaHuntingRealityManager.OnDragonSpawned -= OnDragonSpawned;
            MofaHuntingRealityManager.OnDragonDied -= OnDragonDied;
            MofaHuntingRealityManager.OnTargetLocked -= OnLockTargetSessionEnded;
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
            _dragonControlPanel.gameObject.SetActive(false);
            _monoPanel.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(true);

            OnDragonLockButtonPressed?.Invoke();
        }

        public void OnDragonCancelLockButtonPressedFunc()
        {
            OnLockTargetSessionEnded();

            OnDragonCancelLockButtonPressed?.Invoke();
        }

        private void OnLockTargetSessionEnded()
        {
            _dragonControlPanel.gameObject.SetActive(true);
            _monoPanel.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
        }

        public void OnFlyingSliderValueChanged(float value)
        {
            if (LeanTween.isTweening())
                return;

            if (value > 0.5f)
            {
                // Starts to fly up
                var mofaHuntingRealityManager = HoloKitApp.Instance.RealityManager as MofaHuntingRealityManager;
                var dragonController = mofaHuntingRealityManager.DragonController;
                if (!dragonController.IsFlying)
                {
                    _setFly.Invoke(true);
                }
                _setMovementMobile.Invoke(1f);
                return;
            }

            if (value < 0.5f)
            {
                // Starts to fly down
                _setMovementMobile.Invoke(-1f);
                return;
            }
        }

        public void OnFlyingSliderPointerDown()
        {
            LeanTween.cancelAll();
        }

        public void OnFlyingSliderPointerUp()
        {
            _setMovementMobile.Invoke(0f);
            if (_flyingSlider.value != 0.5f)
            {
                LeanTween.value(_flyingSlider.value, 0.5f, 0.3f)
                    .setDelay(0.1f)
                    .setOnUpdate((float t) =>
                    {
                        _flyingSlider.value = t;
                    });
            }
        }
    }
}
