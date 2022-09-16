using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp
{
    [Serializable]
    public class HoloKitAppLocalPlayerPreferencesData
    {
        public Dictionary<Reality, RealityPreference> RealityPreferences;

        public HoloKitAppLocalPlayerPreferencesData(HoloKitAppLocalPlayerPreferences LocalPlayerPreferences)
        {
            RealityPreferences = LocalPlayerPreferences.RealityPreferences;
        }
    }
}
