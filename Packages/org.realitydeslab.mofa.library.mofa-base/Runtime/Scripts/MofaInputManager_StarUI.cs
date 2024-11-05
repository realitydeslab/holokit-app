// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using RealityDesignLab.Library.HoloKitApp.UI;

namespace RealityDesignLab.MOFA.Library.MOFABase
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
