// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

namespace Holoi.Library.HoloKitApp
{
    [Serializable]
    public class HoloKitAppGlobalSettingsData
    {
        public bool InstructionEnabled = true;

        public bool VibrationEnabled = true;

        public bool PhaseEnabled = true;

        public bool HighResHDREnabled = true;

        public bool RecordMicrophone = true;

        public bool WatermarkEnabled = false;

        public bool UseWifiForMultiplayerEnabled = false;

        public bool ShowTechInfoEnabled = true;

        public Dictionary<string, RealityPreference> RealityPreferences;

        public HoloKitAppGlobalSettingsData(HoloKitAppGlobalSettings globalSettings)
        {
            InstructionEnabled = globalSettings.InstructionEnabled;
            VibrationEnabled = globalSettings.VibrationEnabled;
            PhaseEnabled = globalSettings.PhaseEnabled;
            HighResHDREnabled = globalSettings.HighResHDREnabled;
            RecordMicrophone = globalSettings.RecordMicrophone;
            WatermarkEnabled = globalSettings.WatermarkEnabled;
            UseWifiForMultiplayerEnabled = globalSettings.UseWifiForMultiplayerEnabled;
            ShowTechInfoEnabled = globalSettings.ShowTechInfoEnabled;
            RealityPreferences = globalSettings.RealityPreferences;
        }
    }
}
