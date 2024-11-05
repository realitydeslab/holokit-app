// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public class MofaPoseVisualizationManager : MonoBehaviour
    {
        private void Start()
        {
            HoloKitApp.HoloKitAppMultiplayerManager.OnStartedAdvertising += OnStartedAdvertising;
            HoloKitApp.HoloKitAppMultiplayerManager.OnStoppedAdvertising += OnStoppedAdvertising;
            HoloKitApp.HoloKitAppMultiplayerManager.OnLocalPlayerChecked += OnLocalPlayerChecked;
            HoloKitApp.HoloKitAppMultiplayerManager.OnLocalPlayerRescan += OnLocalPlayerRescan;
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;

            if (HoloKit.HoloKitUtils.IsEditor)
                HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
        }

        private void OnDestroy()
        {
            HoloKitApp.HoloKitAppMultiplayerManager.OnStartedAdvertising -= OnStartedAdvertising;
            HoloKitApp.HoloKitAppMultiplayerManager.OnStoppedAdvertising -= OnStoppedAdvertising;
            HoloKitApp.HoloKitAppMultiplayerManager.OnLocalPlayerChecked -= OnLocalPlayerChecked;
            HoloKitApp.HoloKitAppMultiplayerManager.OnLocalPlayerRescan -= OnLocalPlayerRescan;
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
        }

        private void OnStartedAdvertising()
        {
            HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
        }

        private void OnStoppedAdvertising()
        {
            HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
        }

        private void OnLocalPlayerChecked()
        {
            HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
        }

        private void OnLocalPlayerRescan()
        {
            HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
        }

        private void OnMofaPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Waiting)
            {
                HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
                return;
            }
            
            if (mofaPhase == MofaPhase.Countdown)
            {
                HoloKitApp.HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
                return;
            }
        }
    }
}
