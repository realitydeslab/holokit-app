using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public struct DragonHealthBarParams
    {
        public float PosY;
        public float Width;
        public float Height;
    }

    public class MofaHuntingFightingPanel : MofaFightingPanel
    {
        [SerializeField] private RectTransform _dragonHealthBar;

        private readonly DragonHealthBarParams _monoPortraitDragonHealthBarParams = new()
        {
            PosY = 800f,
            Width = 600f,
            Height = 160f
        };

        private readonly DragonHealthBarParams _monoLandscapeDragonHealthBarParams = new()
        {
            PosY = 800f,
            Width = 2400f,
            Height = 320f
        };

        private readonly DragonHealthBarParams _starDragonHealthBarParams = new()
        {
            PosY = 800f,
            Width = 1800f,
            Height = 240f
        };

        protected override void Start()
        {
            base.Start();
            DragonController.OnDragonSpawned += OnDragonSpawned;
            DragonController.OnDragonDied += OnDragonDied;

            _dragonHealthBar.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DragonController.OnDragonSpawned -= OnDragonSpawned;
            DragonController.OnDragonDied -= OnDragonDied;
        }

        protected override void OnCountdown()
        {
            if (HoloKitApp.Instance.IsSpectator) // Spectator
            {
                //Scores.gameObject.SetActive(true);
                //Time.gameObject.SetActive(true);
            }
            else if (HoloKitApp.Instance.IsHost)
            {
                UpdateDraonHealthBarParams(_monoLandscapeDragonHealthBarParams);
            }
            else
            {
                Reticle.gameObject.SetActive(true);
                Status.gameObject.SetActive(true);
                RedScreen.gameObject.SetActive(true);
            }
        }

        protected override void OnMofaFightingPanelModeChanged(MofaFightingPanelMode mode)
        {
            Debug.Log($"OnMofaFightingPanelModeChanged: {mode}");
            switch (mode)
            {
                case MofaFightingPanelMode.MonoPortrait:
                    UpdateDraonHealthBarParams(_monoPortraitDragonHealthBarParams);
                    break;
                case MofaFightingPanelMode.MonoLandscape:
                    UpdateDraonHealthBarParams(_monoLandscapeDragonHealthBarParams);
                    break;
                case MofaFightingPanelMode.Star:
                    UpdateDraonHealthBarParams(_starDragonHealthBarParams);
                    break;
            }
        }

        private void UpdateDraonHealthBarParams(DragonHealthBarParams healthBarParams)
        {
            _dragonHealthBar.anchoredPosition = new(0f, healthBarParams.PosY);
            _dragonHealthBar.sizeDelta = new(healthBarParams.Width, healthBarParams.Height);
        }

        private void OnDragonSpawned()
        {
            _dragonHealthBar.gameObject.SetActive(true);
        }

        private void OnDragonDied()
        {
            _dragonHealthBar.gameObject.SetActive(false);
        }
    }
}
