using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_TestRealityList : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TestRealityList";

        [SerializeField] private HoloKitAppLocalPlayerPreferences _localPlayerPreferences;

        public void OnEnterRealityAsHost(string realityDisplayName)
        {
            foreach (var reality in _localPlayerPreferences.RealityList.realities)
            {
                if (reality.displayName.Equals(realityDisplayName))
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.EnterRealityAsHost();
                }
            }
        }

        public void OnJoinRealityAsSpectator(string realityDisplayName)
        {
            foreach (var reality in _localPlayerPreferences.RealityList.realities)
            {
                if (reality.displayName.Equals(realityDisplayName))
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.JoinRealityAsSpectator();
                }
            }
        }
    }
}
