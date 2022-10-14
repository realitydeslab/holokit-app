using System;
using System.Collections.Generic;

namespace Holoi.Library.HoloKitApp
{
    [Serializable]
    public class HoloKitAppGlobalSettingsData
    {
        public bool InstructionEnabled;

        public bool VibrationEnabled;

        public bool HighResHDREnabled;

        public bool UseWifiForMultiplayerEnabled;

        public bool ShowTechInfoEnabled;

        public Dictionary<string, RealityPreference> RealityPreferences;

        public HoloKitAppGlobalSettingsData(HoloKitAppGlobalSettings globalSettings)
        {
            InstructionEnabled = globalSettings.InstructionEnabled;
            VibrationEnabled = globalSettings.VibrationEnabled;
            HighResHDREnabled = globalSettings.HighResHDREnabled;
            UseWifiForMultiplayerEnabled = globalSettings.UseWifiForMultiplayerEnabled;
            ShowTechInfoEnabled = globalSettings.ShowTechInfoEnabled;
            RealityPreferences = globalSettings.RealityPreferences;
        }
    }
}
