// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_SERVICES_CORE_ENABLED
using Unity.Services.CloudSave;
#endif
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIPanel_MyAccountPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MyAccountPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _appleIdEmailText;

        private const string AppleUserEmailKey = "AppleUserEmail";

        private void Start()
        {
            if (PlayerPrefs.HasKey(AppleUserEmailKey))
            {
                _appleIdEmailText.text = PlayerPrefs.GetString(AppleUserEmailKey);
            }
            else
            {
                FetchAppleUserEmail();
            }
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        // Try fetching the user's email in the cloud
        private async void FetchAppleUserEmail()
        {
        #if UNITY_SERVICES_CORE_ENABLED
            // We can only fetch from cloud if the user has already signed in
            if (!HoloKitApp.Instance.UserAccountManager.IsSignedIn)
            {
                _appleIdEmailText.text = "Not signed in";
                return;
            };

            var keySet = new HashSet<string> { AppleUserEmailKey };
            try
            {
                Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(keySet);
                if (savedData.ContainsKey(AppleUserEmailKey))
                {
                    var email = savedData[AppleUserEmailKey];
                    PlayerPrefs.SetString(AppleUserEmailKey, email);
                    _appleIdEmailText.text = email;
                    return;
                }
                else
                {
                    _appleIdEmailText.text = "Not found";
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _appleIdEmailText.text = "Connection failed";
                return;
            }
        #endif 
        }
    }
}
