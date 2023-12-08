// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
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

        private const string UserEmailKey = "userEmail";

        private const string UserNameKey = "userName";

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

            HoloKitAppAnalyticsEventManager.OnPlayerRegistered += OnPlayerRegistered;
            HoloKitAppAnalyticsEventManager.OnDreamOver += OnDreamOver;
            HoloKitAppAnalyticsEventManager.OnOverheated += OnOverheated;
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void Analytics_Deinit()
        {
            HoloKitAppAnalyticsEventManager.OnPlayerRegistered -= OnPlayerRegistered;
            HoloKitAppAnalyticsEventManager.OnDreamOver -= OnDreamOver;
            HoloKitAppAnalyticsEventManager.OnOverheated -= OnOverheated;
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

            try
            {
                AnalyticsService.Instance.CustomData("dreamOver", parameters);
            }
            catch (Exception)
            {

            }
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

            try
            {
                AnalyticsService.Instance.CustomData("overheat", parameters);
            }
            catch (Exception)
            {

            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                Dictionary<string, object> parameters = new();

                try
                {
                    AnalyticsService.Instance.CustomData("enterStarMode", parameters);
                }
                catch (Exception)
                {

                }
            }
        }

        private void OnPlayerRegistered(string userEmail, string userName)
        {
            Dictionary<string, object> parameters = new()
            {
                { UserEmailKey, userEmail },
                { UserNameKey, userName }
            };

            try
            {
                AnalyticsService.Instance.CustomData("playerRegistered", parameters);
            }
            catch (Exception)
            {

            }
        }
    }
}
