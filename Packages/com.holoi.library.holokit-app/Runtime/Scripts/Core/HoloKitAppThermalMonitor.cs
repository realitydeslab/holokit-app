using System;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public struct HoloKitAppOverheatData
    {
        public RealitySessionData RealitySessionData;

        public bool IsHost;

        public HoloKitAppPlayerType PlayerType;
    }

    public class HoloKitAppThermalMonitor : MonoBehaviour
    {
        public ThermalState CurrentThermalState => _currentThermalState;

        private ThermalState _currentThermalState;

        public static event Action<HoloKitAppOverheatData> OnOverheated;

        private void Awake()
        {
            _currentThermalState = HoloKitARSessionControllerAPI.GetThermalState();
            HoloKitARSessionControllerAPI.OnThermalStateChanged += OnThermalStateChanged;
        }

        private void OnDestroy()
        {
            HoloKitARSessionControllerAPI.OnThermalStateChanged -= OnThermalStateChanged;
        }

        private void OnThermalStateChanged(ThermalState thermalState)
        {
            if (_currentThermalState == ThermalState.ThermalStateFair && thermalState == ThermalState.ThermalStateSerious)
            {
                if (HoloKitApp.Instance.RealityManager != null)
                {
                    HoloKitAppOverheatData overheatData = new()
                    {
                        RealitySessionData = HoloKitApp.Instance.RealityManager.GetRealitySessionData(),
                        IsHost = HoloKitApp.Instance.IsHost,
                        PlayerType = HoloKitApp.Instance.MultiplayerManager.LocalPlayer.PlayerType.Value
                    };
                    OnOverheated?.Invoke(overheatData);
                }
            }

            _currentThermalState = thermalState;
        }
    }
}