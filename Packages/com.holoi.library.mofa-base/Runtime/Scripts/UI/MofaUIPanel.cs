using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase.UI
{
    public class MofaUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "Mofa";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private RectTransform _triggerButton;

        private const float RotationSpeed = 20f;

        private void Start()
        {
            // Disable the trigger button on spectators
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
                _triggerButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_triggerButton.gameObject.activeSelf)
                _triggerButton.Rotate(new Vector3(0f, 0f, 1f), -RotationSpeed * Time.deltaTime);
        }

        public void OnTriggerButtonPressed()
        {
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
        }
    }
}
