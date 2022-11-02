using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_SettingsPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "SettingsPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _instructionToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _vibrationToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _phaseToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _4khdrToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _wifiToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _techInfoToggle;

        private void Start()
        {
            _instructionToggle.Toggled = HoloKitApp.Instance.GlobalSettings.InstructionEnabled;
            _vibrationToggle.Toggled = HoloKitApp.Instance.GlobalSettings.VibrationEnabled;
            _phaseToggle.Toggled = HoloKitApp.Instance.GlobalSettings.PhaseEnabled;
            _4khdrToggle.Toggled = HoloKitApp.Instance.GlobalSettings.HighResHDREnabled;
            _wifiToggle.Toggled = HoloKitApp.Instance.GlobalSettings.UseWifiForMultiplayerEnabled;
            _techInfoToggle.Toggled = HoloKitApp.Instance.GlobalSettings.ShowTechInfoEnabled;
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnInstructionToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.InstructionEnabled = toggled;
        }

        public void OnVibrationToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.VibrationEnabled = toggled;
        }

        public void OnPhaseToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.PhaseEnabled = toggled;
        }

        public void On4KHDRToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.HighResHDREnabled = toggled;
        }

        public void OnWifiToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.UseWifiForMultiplayerEnabled = toggled;
        }

        public void OnTechInfoToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.ShowTechInfoEnabled = toggled;
        }
    }
}
