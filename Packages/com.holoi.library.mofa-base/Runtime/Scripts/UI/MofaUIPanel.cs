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

        private void Update()
        {
            _triggerButton.Rotate(new Vector3(0f, 0f, 1f), -RotationSpeed * Time.deltaTime);
        }

        public void OnTriggerButtonPressed()
        {
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
        }
    }
}
