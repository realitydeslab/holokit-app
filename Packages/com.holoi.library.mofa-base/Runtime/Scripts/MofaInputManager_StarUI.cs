// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase
{
    public partial class MofaInputManager : MonoBehaviour
    {
        private void InitializeStarUI()
        {
            HoloKitAppUIEventManager.OnStarUITriggered += OnStarUITriggered;
            HoloKitAppUIEventManager.OnStarUIBoosted += OnStarUIBoosted;
        }

        private void DeinitializeStarUI()
        {
            HoloKitAppUIEventManager.OnStarUITriggered -= OnStarUITriggered;
            HoloKitAppUIEventManager.OnStarUIBoosted -= OnStarUIBoosted;
        }

        private void OnStarUITriggered()
        {
            if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Fighting)
                TryCastBasicSpell();
            else if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Waiting)
                _mofaBaseRealityManager.TryGetReady();
        }

        private void OnStarUIBoosted()
        {
            TryCastSecondarySpell();
        }
    }
}
