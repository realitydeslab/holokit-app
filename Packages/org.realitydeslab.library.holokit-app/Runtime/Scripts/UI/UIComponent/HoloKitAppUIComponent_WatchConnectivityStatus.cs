// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using RealityDesignLab.Library.HoloKitApp.WatchConnectivity;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_WatchConnectivityStatus : MonoBehaviour
    {
        [SerializeField] private GameObject _watchAppNotDetected;

        [SerializeField] private GameObject _watchAppNotReachable;

        [SerializeField] private GameObject _watchAppReachable;

        private void OnEnable()
        {
            HoloKitAppWatchConnectivityAPI.OnSessionReachabilityChanged += OnWatchReachabilityChanged;
            if (!HoloKitApp.Instance.CurrentReality.IsAppleWatchRequired())
            {
                OnWatchNotRequired();
                return;
            }
            if (!HoloKitAppWatchConnectivityAPI.DeviceHasPairedAppleWatch())
            {
                OnNoPairedWatch();
                return;
            }
            if (!HoloKitAppWatchConnectivityAPI.IsWatchAppInstalled())
            {
                OnWatchAppNotInstalled();
                return;
            }
            if (!HoloKitAppWatchConnectivityAPI.IsWatchReachable())
            {
                OnWatchNotReachable();
            }
            else
            {
                OnWatchReachable();
            }
        }

        private void OnDisable()
        {
            HoloKitAppWatchConnectivityAPI.OnSessionReachabilityChanged -= OnWatchReachabilityChanged;
        }

        private void OnWatchReachabilityChanged(bool isReachable)
        {
            if (isReachable)
            {
                OnWatchReachable();
            }
            else
            {
                OnWatchNotReachable();
            }
        }

        private void OnWatchNotRequired()
        {
            _watchAppNotDetected.SetActive(false);
            _watchAppNotReachable.SetActive(false);
            _watchAppReachable.SetActive(false);
        }

        private void OnNoPairedWatch()
        {
            _watchAppNotDetected.SetActive(true);
            _watchAppNotReachable.SetActive(false);
            _watchAppReachable.SetActive(false);
        }

        private void OnWatchAppNotInstalled()
        {
            _watchAppNotDetected.SetActive(true);
            _watchAppNotReachable.SetActive(false);
            _watchAppReachable.SetActive(false);
        }

        private void OnWatchNotReachable()
        {
            _watchAppNotDetected.SetActive(false);
            _watchAppNotReachable.SetActive(true);
            _watchAppReachable.SetActive(false);
        }

        private void OnWatchReachable()
        {
            _watchAppNotDetected.SetActive(false);
            _watchAppNotReachable.SetActive(false);
            _watchAppReachable.SetActive(true);
        }
    }
}
