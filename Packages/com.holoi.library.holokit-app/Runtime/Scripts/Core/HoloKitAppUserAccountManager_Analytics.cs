using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Analytics;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// This part of the partial class is responsible for Analytics.
    /// </summary>
    public partial class HoloKitAppUserAccountManager
    {
        private const string RealityBundleIdKey = "realityBundleId";

        private const string SessionDurationKey = "sessionDuration";

        private const string PlayerCountKey = "playerCount";

        private const string IsHostKey = "isHost";

        private const string PlayerTypeKey = "playerType";

        // We cannot initialize Analytics when there is no network connection
        private async void Analytics_Init()
        {
            try
            {
                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            }
            catch (ConsentCheckException e)
            {
                
            }

            HoloKitApp.OnDreamOver += OnDreamOver;
            HoloKitAppThermalMonitor.OnOverheated += OnOverheated;
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void Analytics_Deinit()
        {
            HoloKitApp.OnDreamOver -= OnDreamOver;
            HoloKitAppThermalMonitor.OnOverheated -= OnOverheated;
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void OnDreamOver(RealitySessionData realitySessionData)
        {
            Dictionary<string, object> parameters = new()
            {
                { RealityBundleIdKey, realitySessionData.RealityBundleId },
                { SessionDurationKey, realitySessionData.SessionDuration },
                { PlayerCountKey, realitySessionData.PlayerCount }
            };

            AnalyticsService.Instance.CustomData("dreamOver", parameters);
        }

        private void OnOverheated(HoloKitAppOverheatData overheatData)
        {
            Dictionary<string, object> parameters = new()
            {
                { RealityBundleIdKey, overheatData.RealitySessionData.RealityBundleId },
                { SessionDurationKey, overheatData.RealitySessionData.SessionDuration },
                { PlayerCountKey, overheatData.RealitySessionData.PlayerCount },
                { IsHostKey, overheatData.IsHost },
                { PlayerTypeKey, overheatData.PlayerType }
            };

            AnalyticsService.Instance.CustomData("overheat", parameters);
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            AnalyticsService.Instance.CustomData("enterStarMode", null);
        }
    }
}
