// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using org.realitydeslab.MOFABase;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class MofaHuntingPopupManager : MofaPopupManager
    {
        protected override void OnRoundResult()
        {
            if (HoloKitApp.Instance.IsSpectator)
                return;

            if (HoloKitApp.Instance.IsHost)
            {
                SpawnDefeatPopup();
            }
            else if (HoloKitApp.Instance.IsPlayer)
            {
                SpawnVictoryPopup();
            }
        }

        protected override void OnSummaryBoard()
        {
            
        }
    }
}
