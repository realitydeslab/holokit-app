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

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _recordMicrophoneToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _watermarkEnabledToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _wifiToggle;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _techInfoToggle;

        private void Start()
        {
            var globalSettings = HoloKitApp.Instance.GlobalSettings;
            _instructionToggle.Toggled = globalSettings.InstructionEnabled;
            _vibrationToggle.Toggled = globalSettings.VibrationEnabled;
            _phaseToggle.Toggled = globalSettings.PhaseEnabled;
            _4khdrToggle.Toggled = globalSettings.HighResHDREnabled;
            _recordMicrophoneToggle.Toggled = globalSettings.RecordMicrophone;
            _watermarkEnabledToggle.Toggled = globalSettings.WatermarkEnabled;
            _wifiToggle.Toggled = globalSettings.UseWifiForMultiplayerEnabled;
            _techInfoToggle.Toggled = globalSettings.ShowTechInfoEnabled;
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

        public void OnRecordMicrophoneToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.RecordMicrophone = toggled;
        }

        public void OnWatermarkToggleValueChanged(bool toggled)
        {
            HoloKitApp.Instance.GlobalSettings.WatermarkEnabled = toggled;
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
