// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
#if UNITY_SERVICES_ENABLED
using Unity.Services.Core;
#endif 

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public partial class HoloKitAppUserAccountManager : MonoBehaviour
    {
        /// <summary>
        /// Set this to true to clear session token in the next run.
        /// This is only used for testing purporse.
        /// </summary>
        [SerializeField] private bool _clearSessionToken = false;

        private async void Start()
        {
            if (HoloKitApp.Instance.GlobalSettings.AppConfig?.UserAccountSystemEnabled == true) {
                // UGS can still be initialied when there is no network connection
            #if UNITY_SERVICES_ENABLED
                await UnityServices.InitializeAsync();
                Analytics_Init();
                Authentication_Init();
            #endif
            #if APPLE_SIGNIN_ENABLED
                SIWA_Init();
            #endif
            }
        }

        private void OnDestroy()
        {
            if (HoloKitApp.Instance.GlobalSettings.AppConfig?.UserAccountSystemEnabled == true) {
            #if UNITY_SERVICES_ENABLED
                Analytics_Deinit();
            #endif
            }
        }

        private void Update()
        {
            if (HoloKitApp.Instance.GlobalSettings.AppConfig?.UserAccountSystemEnabled == true) {
            #if APPLE_SIGNIN_ENABLED
                SIWA_Update();
            #endif
            }
        }
    }
}
