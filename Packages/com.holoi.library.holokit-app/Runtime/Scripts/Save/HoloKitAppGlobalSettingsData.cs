using System;
using System.Collections.Generic;

namespace Holoi.Library.HoloKitApp
{
    [Serializable]
    public class HoloKitAppGlobalSettingsData
    {
        public Dictionary<string, RealityPreference> RealityPreferences;

        public HoloKitAppGlobalSettingsData(HoloKitAppGlobalSettings globalSettings)
        {
            RealityPreferences = globalSettings.RealityPreferences;
        }
    }
}
