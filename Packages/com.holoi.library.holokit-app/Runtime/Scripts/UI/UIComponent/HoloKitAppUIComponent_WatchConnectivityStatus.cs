using UnityEngine;
using Holoi.Library.HoloKitApp.WatchConnectivity;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_WatchConnectivityStatus : MonoBehaviour
    {
        [SerializeField] private GameObject _noPairedWatch;

        [SerializeField] private GameObject _watchAppNotInstalled;

        [SerializeField] private GameObject _watchNotReachable;

        [SerializeField] private GameObject _watchReachable;

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
            _noPairedWatch.SetActive(false);
            _watchAppNotInstalled.SetActive(false);
            _watchNotReachable.SetActive(false);
            _watchReachable.SetActive(false);
        }

        private void OnNoPairedWatch()
        {
            _noPairedWatch.SetActive(true);
            _watchAppNotInstalled.SetActive(false);
            _watchNotReachable.SetActive(false);
            _watchReachable.SetActive(false);
        }

        private void OnWatchAppNotInstalled()
        {
            _noPairedWatch.SetActive(false);
            _watchAppNotInstalled.SetActive(true);
            _watchNotReachable.SetActive(false);
            _watchReachable.SetActive(false);
        }

        private void OnWatchNotReachable()
        {
            _noPairedWatch.SetActive(false);
            _watchAppNotInstalled.SetActive(false);
            _watchNotReachable.SetActive(true);
            _watchReachable.SetActive(false);
        }

        private void OnWatchReachable()
        {
            _noPairedWatch.SetActive(false);
            _watchAppNotInstalled.SetActive(false);
            _watchNotReachable.SetActive(false);
            _watchReachable.SetActive(true);
        }
    }
}
