// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    public class MofaPoseVisualizationManager : MonoBehaviour
    {
        private void Start()
        {
            HoloKitAppMultiplayerManager.OnStartedAdvertising += OnStartedAdvertising;
            HoloKitAppMultiplayerManager.OnStoppedAdvertising += OnStoppedAdvertising;
            HoloKitAppMultiplayerManager.OnLocalPlayerChecked += OnLocalPlayerChecked;
            HoloKitAppMultiplayerManager.OnLocalPlayerRescan += OnLocalPlayerRescan;
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;

            if (HoloKit.HoloKitUtils.IsEditor)
                HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnStartedAdvertising -= OnStartedAdvertising;
            HoloKitAppMultiplayerManager.OnStoppedAdvertising -= OnStoppedAdvertising;
            HoloKitAppMultiplayerManager.OnLocalPlayerChecked -= OnLocalPlayerChecked;
            HoloKitAppMultiplayerManager.OnLocalPlayerRescan -= OnLocalPlayerRescan;
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
        }

        private void OnStartedAdvertising()
        {
            HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
        }

        // The labels are hidden from the countdown until the round is over. A host who stops showing its QR code, or a
        // spectator who joins, during a round must not bring them back.
        private static bool BetweenRounds =>
            HoloKitApp.Instance.RealityManager is not MofaBaseRealityManager manager ||
            manager.CurrentPhase.Value == MofaPhase.Waiting;

        private void OnStoppedAdvertising()
        {
            HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = BetweenRounds;
        }

        private void OnLocalPlayerChecked()
        {
            HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = BetweenRounds;
        }

        private void OnLocalPlayerRescan()
        {
            HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
        }

        private void OnMofaPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Waiting)
            {
                HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
                return;
            }
            
            if (mofaPhase == MofaPhase.Countdown)
            {
                HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = false;
                return;
            }
        }
    }
}
