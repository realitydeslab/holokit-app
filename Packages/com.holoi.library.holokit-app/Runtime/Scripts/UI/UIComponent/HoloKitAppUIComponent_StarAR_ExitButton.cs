using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_ExitButton : HoloKitAppUITemplate_StarAR_HorizontalScrollButton
    {
        public override bool SwipeRight => true;

        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        protected override void OnSelected()
        {
            base.OnSelected();
            _starARPanel.OnExitButtonPressed();
        }

        protected override void OnRecovered()
        {
            base.OnRecovered();
            _starARPanel.OnExitButtonReleased();
        }

        protected override void OnTriggerred()
        {
            base.OnTriggerred();
            Screen.orientation = ScreenOrientation.Portrait;
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Mono);
        }
    }
}
